using UnityEngine;
using CW.Common;
using Lean.Common;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to translate the current GameObject relative to the camera using the finger drag gesture.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTranslate")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate")]
	public class LeanDragTranslate : MonoBehaviour
	{
        [System.Serializable] public class StartDragEvent : UnityEvent { }
        [System.Serializable] public class EndDragEvent : UnityEvent { }
        [System.Serializable] public class IDragOverHandlerEvent : UnityEvent<IDragOverHandler> { }

        public float Threshold = 1;
        public LeanScreenQuery ScreenQuery = new LeanScreenQuery(LeanScreenQuery.MethodType.Raycast);
        /// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
        public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera the translation will be calculated using.
		/// None/null = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [SerializeField] private Camera _camera;

		/// <summary>The movement speed will be multiplied by this.
		/// -1 = Inverted Controls.</summary>
		public float Sensitivity { set { sensitivity = value; } get { return sensitivity; } } [SerializeField] private float sensitivity = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [SerializeField] protected float damping = -1.0f;

		/// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
		/// NOTE: This requires <b>Dampening</b> to be above 0.</summary>
		public float Inertia { set { inertia = value; } get { return inertia; } } [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

		[SerializeField]
		private Vector3 remainingTranslation;

        public StartDragEvent OnStartDragHandler { get { if(onStartDragHandler == null) onStartDragHandler = new StartDragEvent(); return onStartDragHandler; } }
        [SerializeField] private StartDragEvent onStartDragHandler;
        public EndDragEvent OnEndDragHandler { get { if(onEndDragHandler == null) onEndDragHandler = new EndDragEvent(); return onEndDragHandler; } }
        [SerializeField] private EndDragEvent onEndDragHandler;
        public IDragOverHandlerEvent OnDragOverHandlerEvent { get { if(onDrapOverHandler == null) onDrapOverHandler = new IDragOverHandlerEvent(); return onDrapOverHandler; } }
        [SerializeField] private IDragOverHandlerEvent onDrapOverHandler;

        bool isDragging = false;

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
        public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}


   
		protected virtual void Update()
		{
			// Store
			var oldPosition = transform.localPosition;

			// Get the fingers we want to use
			var fingers = Use.UpdateAndGetFingers();
            

			// Calculate the screenDelta value based on these fingers
			var screenDelta = LeanGesture.GetScreenDelta(fingers);

			if (screenDelta != Vector2.zero)
			{
				// Perform the translation
				if (transform is RectTransform)
				{
					TranslateUI(screenDelta);
				}
				else
				{
					Translate(screenDelta);
				}
                /*
                if(isDragging == false)
                {
                    float moveDistance = Vector3.Distance(transform.localPosition, oldPosition);
                    if(moveDistance > Threshold)
                    {
                        isDragging = true;
                        OnStartDragHandler?.Invoke();
                    }
                }
                */
            }
            else
            {
                /*
                if(isDragging)
                {
                    isDragging = false;
                    OnEndDragHandler?.Invoke();
                }
                */
            }

            if(fingers == null || fingers.Count == 0)
            {
                if(isDragging)
                {
                    isDragging = false;
                    OnEndDragHandler?.Invoke();
                }
              
            }
            else
            {
                if(isDragging == false)
                {
                    float moveDistance = Vector3.Distance(transform.localPosition, oldPosition);
                    if(moveDistance > Threshold)
                    {
                        isDragging = true;
                        OnStartDragHandler?.Invoke();
                    }

                    //isDragging = true;
                   // OnStartDragHandler?.Invoke();
                }
                for(int i = 0; i < fingers.Count; i++)
                {
                    var drapOverHandler = default(IDragOverHandler);
                    var root = default(Component);
                    var worldPosition = default(Vector3);
                    var query = ScreenQuery.TryQuery(gameObject, fingers[i].ScreenPosition, ref drapOverHandler, ref root, ref worldPosition);
                    if(query == true)
                    {
                        drapOverHandler.HandleOver(gameObject, fingers[i]);
                    }
                }
                
            }


            // Increment
            remainingTranslation += transform.localPosition - oldPosition;

			// Get t value
			var factor = CwHelper.DampenFactor(Damping, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingTranslation = Vector3.Lerp(remainingTranslation, Vector3.zero, factor);

			// Shift this transform by the change in delta
			transform.localPosition = oldPosition + remainingTranslation - newRemainingTranslation;

			if (fingers.Count == 0 && inertia > 0.0f && Damping > 0.0f)
			{
				newRemainingTranslation = Vector3.Lerp(newRemainingTranslation, remainingTranslation, inertia);
			}

			// Update remainingDelta with the dampened value
			remainingTranslation = newRemainingTranslation;
		}

		private void TranslateUI(Vector2 screenDelta)
		{
			var finalCamera = _camera;

			if (finalCamera == null)
			{
				var canvas = transform.GetComponentInParent<Canvas>();

				if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
				{
					finalCamera = canvas.worldCamera;
				}
			}

			// Screen position of the transform
			var screenPoint = RectTransformUtility.WorldToScreenPoint(finalCamera, transform.position);

			// Add the deltaPosition
			screenPoint += screenDelta * Sensitivity;

			// Convert back to world space
			var worldPoint = default(Vector3);

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent as RectTransform, screenPoint, finalCamera, out worldPoint) == true)
			{
				transform.position = worldPoint;
			}
		}

		private void Translate(Vector2 screenDelta)
		{
			// Make sure the camera exists
			var camera = CwHelper.GetCamera(this._camera, gameObject);

			if (camera != null)
			{
				// Screen position of the transform
				var screenPoint = camera.WorldToScreenPoint(transform.position);

				// Add the deltaPosition
				screenPoint += (Vector3)screenDelta * Sensitivity;

				// Convert back to world space
				transform.position = camera.ScreenToWorldPoint(screenPoint);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanDragTranslate;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET), true)]
	public class LeanDragTranslate_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Use");
			Draw("_camera", "The camera the translation will be calculated using.\n\nNone/null = MainCamera.");
			Draw("sensitivity", "The movement speed will be multiplied by this.\n\n-1 = Inverted Controls.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("inertia", "This allows you to control how much momentum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Damping</b> to be above 0.");
            Draw("Threshold");
            Draw("ScreenQuery");
            Separator();
            var usedA = Any(tgts, t => t.OnStartDragHandler.GetPersistentEventCount() > 0);
            var usedB = Any(tgts, t => t.OnEndDragHandler.GetPersistentEventCount() > 0);
            var usedC = Any(tgts, t => t.OnDragOverHandlerEvent.GetPersistentEventCount() > 0);

            var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

            if(usedA == true || showUnusedEvents == true)
            {
                Draw("onStartDragHandler");
            }

            if(usedB == true || showUnusedEvents == true)
            {
                Draw("onEndDragHandler");
            }

            if(usedC == true || showUnusedEvents == true)
            {
                Draw("onDrapOverHandler");
            }
        }
	}
}
#endif