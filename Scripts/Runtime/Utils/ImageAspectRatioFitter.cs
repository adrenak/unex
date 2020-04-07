namespace UnityEngine.UI {
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class ImageAspectRatioFitter : MonoBehaviour {
        AspectRatioFitter fitter;
        Image image;
        Texture2D tex;

        void Awake(){
            Init();
        }

        void OnValidate() {
            Init();
        }

        void Init(){
            fitter = GetComponent<AspectRatioFitter>();
            image = GetComponent<Image>();
        }

        void Update() {
            if (image.sprite == null) return;

            var t = image.sprite.texture;
            if (t == tex)
                return;
            tex = t;
            fitter.aspectRatio = (float) t.width / t.height;
        }
    }
}
