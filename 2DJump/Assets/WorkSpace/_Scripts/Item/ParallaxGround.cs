using UnityEngine;

public class ParallaxGround : MonoBehaviour
{
    private Sprite sprite;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private Transform cameraTransform;
    private Vector3 LastFramePos;
    private Vector2 movement;
    [SerializeField] private Vector2 parallaxSpeed;

    private float intervalTime;

    private Transform player => GameObject.Find("Player").transform;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        cameraTransform = Camera.main.transform;
    }
    private void OnEnable()
    {
        EventHandler.AddEventListener("AfterSceneLoadEvent", ResetPosition);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener("AfterSceneLoadEvent", ResetPosition);
    }


    private void Start()
    {
        textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = sprite.texture.height / sprite.pixelsPerUnit;
    }

    private void Update()
    {
        movement = cameraTransform.position - LastFramePos;
        transform.position += new Vector3(movement.x * parallaxSpeed.x, movement.y * parallaxSpeed.y, 0);
        LastFramePos = cameraTransform.position;

        intervalTime -= Time.deltaTime;

        if (intervalTime <= 0)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) > textureUnitSizeX - 10f)
            {
                float offsetX = cameraTransform.position.x - transform.position.x;
                transform.position = new Vector3(cameraTransform.position.x + offsetX, transform.position.y, 0);
                intervalTime = 2f;
            }
            //if (Mathf.Abs(cameraTransform.position.y - transform.position.y) > textureUnitSizeY - 10f)
            //{
            //    float offsetY = cameraTransform.position.y - transform.position.y;
            //    transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetY, 0);
            //    intervalTime = 2f;
            //}
        }
    }

    private void ResetPosition()
    {
        cameraTransform.position = new Vector3(player.transform.position.x, 0, 0);
        LastFramePos = cameraTransform.position;
        movement = cameraTransform.position - LastFramePos;
        transform.position = new Vector3(player.transform.position.x, 0, 0);
        intervalTime = 2f;
    }
}
