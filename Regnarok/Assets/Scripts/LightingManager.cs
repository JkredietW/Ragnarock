using UnityEngine;
using Photon.Pun;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    public float devideDay;
    public float devideNight;
    public bool isNight;
    private GameManager gm;


	private void Start()
	{
        gm = GetComponent<GameManager>();
        RenderSettings.fog = false;
        //GetComponent<PhotonView>().RPC("SyncTime", RpcTarget.All, false, TimeOfDay);
    }
	private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
			if (TimeOfDay <= 5|| TimeOfDay>=18.5f)
			{
                TimeOfDay += Time.deltaTime/ devideNight;
				if (!isNight)
				{
                    isNight = true;
                    if (!gm.isDoingNight)
					{
                        if (PhotonNetwork.IsMasterClient)
                        {
                            gm.StartCoroutine("IsNight");
                            GetComponent<PhotonView>().RPC("SyncTime", RpcTarget.All, true, TimeOfDay);
                        }
					}
                }
			}
			else
			{
                TimeOfDay += Time.deltaTime / devideDay;
                if (isNight)
                {
					if (gm.nightAudio.isPlaying)
					{
						gm.dayAudio.Play();
						gm.nightAudio.Stop();
					}
					isNight = false;
                    gm.isDoingNight = false;
                    RenderSettings.fog = false;
                }
            }
            TimeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }
    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
    [PunRPC]
    public void SyncTime(bool b,float f)
	{
		if (b)
		{
            RenderSettings.fog = true;
        }
		else
		{
            RenderSettings.fog = false;
        }
        TimeOfDay = f;
    }
}