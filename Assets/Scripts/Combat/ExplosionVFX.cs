using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the explosion visual effect using a sprite sheet.
/// Automatically loads the "VFX/Explosion" resource if not assigned.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionVFX : MonoBehaviour
{
    [SerializeField] private Sprite[] frames; // Can be assigned manually or loaded
    [SerializeField] private float duration = 0.5f; // Total duration of explosion
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // If frames are not assigned, try to generate them from the texture in Resources
        if (frames == null || frames.Length == 0)
        {
            LoadFramesFromResources();
        }

        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("ExplosionVFX: No frames found!");
            Destroy(gameObject);
            yield break;
        }

        float frameRate = duration / frames.Length;
        
        for (int i = 0; i < frames.Length; i++)
        {
            sr.sprite = frames[i];
            yield return new WaitForSeconds(frameRate);
        }

        Destroy(gameObject);
    }

    private void LoadFramesFromResources()
    {
        // Load the texture
        Texture2D texture = Resources.Load<Texture2D>("VFX/Explosion");
        if (texture == null)
        {
            Debug.LogError("ExplosionVFX: Could not load 'VFX/Explosion' from Resources.");
            return;
        }

        // Slice the texture into sprites
        int frameWidth = texture.width / columns;
        int frameHeight = texture.height / rows;
        frames = new Sprite[rows * columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // Invert Y because texture coordinates start from bottom-left, but grid usually reads top-left
                // Actually, standard sprite sheets are often read top-left to bottom-right.
                // Let's assume standard reading order: Row 0 is top.
                // Texture coords: (0,0) is bottom-left.
                // So Row 0 (top) corresponds to y index (rows - 1 - y) in texture space.
                
                int texX = x * frameWidth;
                int texY = (rows - 1 - y) * frameHeight;

                Rect rect = new Rect(texX, texY, frameWidth, frameHeight);
                frames[y * columns + x] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }
        }
    }
}
