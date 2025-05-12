using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace SimpleFileBrowser
{
	public class FileBrowserRenamedItem : MonoBehaviour
	{
		public delegate void OnRenameCompleted( string filename );

#pragma warning disable 0649
		[SerializeField]
		private Image background;

		[SerializeField]
		private Image icon;

		[SerializeField]
		private InputField nameInputField;
		public InputField InputField { get { return nameInputField; } }
#pragma warning restore 0649

		private OnRenameCompleted onRenameCompleted;

		private RectTransform m_transform;
		public RectTransform TransformComponent
		{
			get
			{
				if( m_transform == null )
					m_transform = (RectTransform) transform;

				return m_transform;
			}
		}

		public void Show( string initialFilename, Color backgroundColor, Sprite icon, OnRenameCompleted onRenameCompleted )
		{
			background.color = backgroundColor;
			this.icon.sprite = icon;
			this.onRenameCompleted = onRenameCompleted;

			transform.SetAsLastSibling();
			gameObject.SetActive( true );

			nameInputField.text = initialFilename;
			nameInputField.ActivateInputField();
		}


		private void LateUpdate()
		{
			// Don't allow scrolling with mouse wheel while renaming a file or creating a folder
			if(Mouse.current != null && Mouse.current.scroll.ReadValue().y != 0f )
				nameInputField.DeactivateInputField();
		}

		public void OnInputFieldEndEdit( string filename )
		{
			gameObject.SetActive( false );

			// If we don't deselect the InputField manually, FileBrowser's keyboard shortcuts
			// no longer work until user clicks on a UI element and thus, deselects the InputField
			if( !EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject == nameInputField.gameObject )
				EventSystem.current.SetSelectedGameObject( null );

			if( onRenameCompleted != null )
				onRenameCompleted( filename );
		}
	}
}