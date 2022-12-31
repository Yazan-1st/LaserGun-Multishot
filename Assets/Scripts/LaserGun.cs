using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    Camera cam;

    [Header("Bullets")]
    [Header(" -Shoot:")]

    public List<GameObject> bulletHolesList;
    public int bulletHoleIndex;
    private GameObject bulletHole;

    public float bulletRange = 10;

    public int shotsPerSecond = 12;
    private float timeTillShoot = 1f;

    public int bulletsPerShot;

    public float bulletsSpreadAmount;

    public float minBulletsDestroyTime;
    public float maxBulletsDestroyTime;

    private float bulletsDestroyTime;


    [Header(" -Scan:")]
    public float fullScanDelay;
    private float fullScanDelayTime;
    /*
        private float timeTillScan = 1f;
        private float oldTimeTillScan = 1f;*/

    public int bulletsPerScan;

    public float scanSpreadAmount;


    public float RaycastDir_Y;

    [Header("")]

    public LayerMask canBeShot;                 // check if the object can be shot to avoid player


    [Header("Laser")]

    public GameObject laser;
    public Transform muzzle;                    // get the position where the laser will come from
    public float fadeDuration = 0.09f;           //laser fade duration


    [Header("Audio")]

    public AudioSource LaserBeam;
    public AudioSource LaserBeam2;

    public AudioSource LaserScan;

    public AudioSource GasLeak;



    void Start()
    {
        //bulletHolesList = new List<GameObject>(Resources.LoadAll<GameObject>("BulletHolePrefabs"));

        bulletHoleIndex = Random.Range(0, bulletHolesList.Count);
        Debug.Log(bulletHolesList.Count);
        Debug.Log(bulletHoleIndex);

        bulletHole = bulletHolesList[bulletHoleIndex];

        cam = Camera.main;
        fullScanDelayTime = fullScanDelay;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {   if (bulletHoleIndex == 3)
                bulletHoleIndex = 0;
            else
                bulletHoleIndex += 1;
        }

        bulletHole = bulletHolesList[bulletHoleIndex];

        bulletsDestroyTime = Random.Range(minBulletsDestroyTime, maxBulletsDestroyTime);

        /////////////ScrollWheel Inputs/////////////

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && bulletsSpreadAmount <= 20 && bulletsSpreadAmount >= 1)
        {
            bulletsSpreadAmount -= 1;
            if (bulletsSpreadAmount <= 10 && bulletsSpreadAmount >=1)
            {
                LaserBeam.pitch -= 0.1f;
                LaserBeam2.pitch -= 0.1f;
            }
        }

        if (bulletsSpreadAmount <= 1)  // Check if the bulletSpread Amount is less than 1 to avoid losing the scrollwheel input
            bulletsSpreadAmount = 1;

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && bulletsSpreadAmount <= 20 && bulletsSpreadAmount >= 1)
        {
            bulletsSpreadAmount += 1;
            if (bulletsSpreadAmount > 10 && bulletsSpreadAmount <= 20)
            {
                LaserBeam.pitch = -1;
                LaserBeam2.pitch = -1;
            }
            if (bulletsSpreadAmount <= 10 && bulletsSpreadAmount >= 1)
            {
                LaserBeam.pitch += 0.1f;
                LaserBeam2.pitch += 0.1f;

            }


        }

        if (bulletsSpreadAmount >= 20)  // Check if the bulletSpread Amount is more than 20 to avoid losing the scrollwheel input
            bulletsSpreadAmount = 20;

        ////////////////////////////////////////////

        /////////////Mouse Inputs///////////////////

        if (Input.GetKey(KeyCode.Mouse0) && timeTillShoot <= 0)
        {
            //FinishedPlayingSound = true;
            //LaserBeam.Play();
            //LaseBeamOBJ.SetActive(true);

            Shoot();
            if (!LaserBeam.isPlaying || !LaserBeam2.isPlaying)
                StartCoroutine(WaitForSoundToFinish());

            
        }

        if (!Input.GetKey(KeyCode.Mouse0))
        {
            LaserBeam.Stop();
            LaserBeam2.Stop();
            //Debug.Log("Stopped Playing both sounds");
        }

        

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LaserBeam.Stop();
            LaserBeam2.Stop();
            //Debug.Log("Stopped Playing both sounds");
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && fullScanDelay <= 0)
        {
            //LaseBeamOBJ.SetActive(false);
            LaserScan.Play();

            fullScanDelay = fullScanDelayTime;
            StartCoroutine(WaitForTenthofSecond());

            RaycastDir_Y = 10f;
        }

        ////////////////////////////////////////////


        else
        {
            timeTillShoot -= shotsPerSecond * Time.deltaTime;
            fullScanDelay -= 1 * Time.deltaTime;
        }

    }

    void Shoot()
    {
        for (int i = 1; i <= bulletsPerShot; i++)
        {
            RaycastHit hit;
            Vector3 shootingDir = GetShootingDirection();

            if (Physics.Raycast(cam.transform.position, shootingDir, out hit, bulletRange, canBeShot))
            {
                if(hit.collider.tag != "RenderOBJ" && hit.collider.tag != "Enemy")
                {
                    GameObject newHole = Instantiate(bulletHole, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));

                    Destroy(newHole, bulletsDestroyTime);

                    CreateLaser(hit.point);
                }

                if (hit.collider.tag == "Enemy")
                {
                    GasLeak.Play();
                }
                else
                {
                    GasLeak.Stop();
                }
            }


            else
            {
                CreateLaser(cam.transform.position + shootingDir * bulletRange);
            }
        }

        timeTillShoot = 1f;
    }

    void Scan()
    {
        for (int c = 1; c <= bulletsPerScan; c++)
        {
            RaycastHit hit;
            Vector3 scanDir = GetScanDirection();

            //Debug.Log(scanDir.y);

            if (Physics.Raycast(cam.transform.position, scanDir, out hit, bulletRange, canBeShot))
            {
                if (hit.collider.tag != "RenderOBJ" && hit.collider.tag != "Enemy")
                {
                    GameObject newHole = Instantiate(bulletHole, hit.point + (hit.normal * .001f), Quaternion.LookRotation(hit.normal));

                    Destroy(newHole, bulletsDestroyTime);

                    if (Random.Range(1, 2) == 1)
                        CreateLaser(hit.point);
                }

                if (hit.collider.tag == "Enemy")
                {
                    GasLeak.Play();
                }
            }


            else
            {
                CreateLaser(cam.transform.position + scanDir * bulletRange);
            }

        }

        //RaycastDir_Y -= 1 * Time.deltaTime;


        //RaycastDir_Y = 10f;

        timeTillShoot = 1f;

    }


    Vector3 GetShootingDirection()
    {
        Vector3 targetPos = cam.transform.position + cam.transform.forward * bulletRange;
        targetPos = new Vector3(targetPos.x + Random.Range(-bulletsSpreadAmount, bulletsSpreadAmount),
                                targetPos.y + Random.Range(-bulletsSpreadAmount, bulletsSpreadAmount),
                                targetPos.z + Random.Range(-bulletsSpreadAmount, bulletsSpreadAmount));
        Vector3 direction = targetPos - cam.transform.position;

        return direction.normalized;
    }

    Vector3 GetScanDirection()
    {
        Vector3 targetPos = cam.transform.position + cam.transform.forward * bulletRange;

        targetPos = new Vector3(targetPos.x + Random.Range(-scanSpreadAmount, scanSpreadAmount),
                                targetPos.y + RaycastDir_Y,
                                targetPos.z + Random.Range(-scanSpreadAmount, scanSpreadAmount));
        Vector3 direction = targetPos - cam.transform.position;

        //Debug.Log(direction.y);

        return direction.normalized;
    }

    void CreateLaser(Vector3 endPos)
    {
        LineRenderer lr = Instantiate(laser).GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[2] { muzzle.position, endPos });

        StartCoroutine(FadeLaser(lr,endPos));

    }

    IEnumerator FadeLaser(LineRenderer lr, Vector3 endPos)
    {
        float alpha = 1;
        while (alpha > 0 && lr != null)
        {
            alpha -= Time.deltaTime / fadeDuration;
            //lr.startColor = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, alpha);
            //lr.endColor = new Color(lr.endColor.r, lr.endColor.g, lr.endColor.b, alpha);

            lr.SetPositions(new Vector3[2] { muzzle.position, endPos });

            yield return null;
        }
    }

    IEnumerator WaitForTenthofSecond()
    {

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;

        yield return new WaitForSeconds(.1f);

        //Debug.Log("finished waiting");

        Scan();

        RaycastDir_Y -= 0.5f;





    }

    IEnumerator WaitForSoundToFinish()
    {
        //Debug.Log("Started playing sound 1");

        //yield return new WaitForSeconds(0.052f);
        LaserBeam.Play();

        yield return new WaitForSeconds(0.052f);
        //Debug.Log("Started Playying sound 2");

        LaserBeam2.Play();


    }

}
