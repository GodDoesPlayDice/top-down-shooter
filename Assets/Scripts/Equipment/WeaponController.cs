using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ShootingModes
{
    Automatic,
    Single,
}


public class WeaponController : MonoBehaviour
{
    public float ownerAgility = 1f;
    public float ownerRigControllerAgility = 1f;
    public GameObject ownerObjRef;


    public float shootRange = 100f;
    public float shotDamage = 0f;
    public float shotImpactForce = 100f;
    public float recoilHeight = 1f;
    public int clipSize = 5;
    public float rateOfFire = 0f;
    public float reloadTime = 0f;
    public ShootingModes shootingMode = ShootingModes.Single;


    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale;

    public GameObject bulletFlashRef;
    public GameObject bulletOutPointObj;

    public GameObject hitEffectRef;

    public GameObject bulletObjRef;
    public float bulletVelocity = 30f;
    public int bulletsInClip = 0;

    public float maxShotAngle = 10f;

    private PlayerController playerController;
    private ShootingEnemyController enemyController;

    private GameObject aimAtPoint;
    public float reloadTimer { get; private set; } = 0f;
    private float nextShotTimer = 0f;

    private GameObject mainUIRef;
    private int amountOfBullets;
    private void Start()
    {
        playerController = ownerObjRef.GetComponent<PlayerController>();
        enemyController = ownerObjRef.GetComponent<ShootingEnemyController>();



        // установка дефолтных значений таймеров и патронов в обойме
        bulletsInClip = clipSize;
        reloadTimer = 0f;
        nextShotTimer = 0f;

        // поиск объекта UI
        mainUIRef = GameObject.FindGameObjectWithTag("MainUI");


        if (playerController != null)
        {
            amountOfBullets = playerController.amountOfBullets;
        }
        else if (enemyController != null)
        {
            amountOfBullets = enemyController.amountOfBullets;
        }
    }

    private void Update()
    {

        // таймеры
        if (reloadTimer > 0f)
        {
            reloadTimer -= Time.deltaTime;
            //playerRef.GetComponent<PlayerController>().isAiming = playerRef.GetComponent<PlayerController>().alwaysAiming;
        }
        if (nextShotTimer > 0f)
        {
            nextShotTimer -= Time.deltaTime;
        }
        if (reloadTimer < 0f)
        {
            reloadTimer = 0f;
            //playerRef.GetComponent<PlayerController>().isAiming = false;
        }
        if (nextShotTimer < 0f)
        {
            nextShotTimer = 0f;
        }

        // если патронов в магазине не осталось, происходит автоматическая перезарядка
        if (bulletsInClip < 1 && amountOfBullets > 0)
        {
            Reload();
        }
    }


    // вспомогательный метод для поиска вложенного объекта по имени с проходом по всем вложенным объектам
    private void FindInAllChildren(Transform obj, string name, ref GameObject storeInObj)
    {
        if (obj.Find(name) != null)
        {
            storeInObj = obj.Find(name).gameObject;
        }
        else
        {
            foreach (Transform eachChild in obj)
            {
                if (eachChild.childCount > 0)
                {
                    FindInAllChildren(eachChild, name, ref storeInObj);
                }
            }
        }

    }


    public void Shoot(float shotDamageModifier, Vector3 targetPos)
    {
        // если один из таймеров еще не достиг нуля, выстрел невозможен
        if (reloadTimer > 0f || nextShotTimer > 0f) return;
        // если в обойме нет патронов, вместо выстрела производится перезарядка
        if (bulletsInClip <= 0)
        {
            Reload();
            return;
        }

        ShotRecoil();

        if (bulletsInClip >= 1)
        {
            //Debug.DrawLine(pos, targetPos, Color.green, Mathf.Infinity);
            // частицы у ствола
            if (bulletFlashRef != null)
                Instantiate(bulletFlashRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);

            // убираем пулю из обоймы
            bulletsInClip -= 1;

            GameObject instantiatedProjectile = Instantiate(bulletObjRef, bulletOutPointObj.transform.position, bulletOutPointObj.transform.rotation);

            var bulletController = instantiatedProjectile.GetComponent<BulletController>();

            // данные об уроне, силе толчка и эффекте на поверхности, куда попала пуля
            bulletController.bulletImpactForce = shotImpactForce;
            bulletController.shotDamage = shotDamage + shotDamageModifier;
            bulletController.hitEffectRef = hitEffectRef;
            bulletController.shooter = ownerObjRef;

            // скорость и направление полета пули
            Vector3 shotDir = targetPos - bulletOutPointObj.transform.position;
            shotDir.y = 0.0f;
            shotDir = shotDir.normalized * bulletVelocity;
            shotDir = GetScatterAngle() * shotDir;
            bulletController.SetVelocity(shotDir);
            instantiatedProjectile.transform.rotation = Quaternion.LookRotation(Vector3.up, shotDir);
        }
        // таймер времени на выстрел
        nextShotTimer = rateOfFire;
    }

    public Quaternion GetScatterAngle()
    {
        float halfMaxAngle = maxShotAngle / 2;
        float angle1 = Random.Range(0 - halfMaxAngle, halfMaxAngle);
        float angle2 = Random.Range(0 - halfMaxAngle, halfMaxAngle);
        var result = Quaternion.Euler(0f, Mathf.Abs(angle1) < Mathf.Abs(angle2) ? angle1 : angle2, 0f);
        return result;
    }

    public void ShotRecoil()
    {
        FindInAllChildren(ownerObjRef.transform, "AimAtPoint", ref aimAtPoint);
        if (aimAtPoint != null)
        {
            // направление отдачи
            Vector3 offset = new Vector3(0f, recoilHeight, 0f);
            aimAtPoint.transform.position = aimAtPoint.transform.position + offset;
        }
    }


    public void Reload()
    {
        // невозможно перезарядиться, если уже идет перезарядка
        if (reloadTimer > 0f) return;
        // устанавливаем таймер
        reloadTimer = reloadTime;

        int needToAdd = clipSize - bulletsInClip;


        // когда патроны остались только в магазине, перезарядка невозможна
        if (amountOfBullets <= 0f) return;


        // если происходит перезарядка полностью израсходованного магазина
        if (needToAdd == clipSize)
        {
            // когда общее кол-во патронов больше размера магазина
            if (amountOfBullets >= clipSize)
            {
                bulletsInClip = clipSize;
            }
            // когда общее кол-во патронов меньше размера магазина
            else
            {
                bulletsInClip = amountOfBullets;
            }

            if (playerController != null)
            {
                playerController.amountOfBullets -= bulletsInClip;
            }
            if (enemyController != null)
            {
                enemyController.amountOfBullets -= bulletsInClip;
            }


        }
        // если происходит перезарядка полностью израсходованного магазина
        else
        {
            // когда общее кол-во патронов больше размера магазина
            if (amountOfBullets >= needToAdd)
            {
                bulletsInClip += needToAdd;

                if (playerController != null)
                {
                    playerController.amountOfBullets -= needToAdd;
                }
                if (enemyController != null)
                {
                    enemyController.amountOfBullets -= needToAdd;
                }
            }
            // когда общее кол-во патронов меньше размера магазина
            else
            {
                bulletsInClip += amountOfBullets;

                if (playerController != null)
                {
                    playerController.amountOfBullets = 0;
                }
                if (enemyController != null)
                {
                    enemyController.amountOfBullets = 0;
                }
            }
        }


        if (playerController != null)
        {
            amountOfBullets = playerController.amountOfBullets;
        }
        if (enemyController != null)
        {
            amountOfBullets = enemyController.amountOfBullets;
        }
    }
}