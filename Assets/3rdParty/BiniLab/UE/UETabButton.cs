/*********************************************
 * NHN StarFish - UI Extends
 * CHOI YOONBIN
 * 
 *********************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[System.Serializable]
public class UETabEvent : UnityEvent<int> {}

public class UETabButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler
{

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// public

	//IPointerClickHandler
	public void OnPointerClick( PointerEventData eventData )
	{
		onTab.Invoke(this.index);
		transform.localScale = startScale;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		transform.localScale = startScale * 1.1f;
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		transform.localScale = startScale;
	}

	public void SetSelected(bool isSelected)
	{
		this.isSelected = isSelected;
		this.UpdateUI ();
	}

	public void SetIndex(int index)
	{
		this.index = index;
	}

	public void AddTapClickEvent(UnityAction<int> evt)
	{
		this.onTab.AddListener(evt);
	}

	public GameObject GetNotiObj()
	{
		return notiObj;
	}

	public void SetButtonResource(string token, Sprite image)
	{
		_text.ToLocalize(token);
		_image.sprite = image;

		if (_text2 != null)
			_text2.ToLocalize(token);
		if (_image2 != null)
			_image2.sprite = image;
	}

	public void SetText(string token, params object[] args)
	{
		_text.ToLocalize(token, args);
		if (_text2 != null)
			_text2.ToLocalize(token, args);
	}
	
	public int Index { get { return this.index; } }
	public bool IsSelected { get { return this.isSelected; } }
	public GameObject DimmedObj => _dimmedObj;

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Life Cycle

	void Start()
	{
		this.UpdateUI ();
		startScale = transform.localScale;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	// private

	[SerializeField] private GameObject enabledObj;
	[SerializeField] private GameObject disabledObj;
	[SerializeField] private GameObject notiObj;

	[SerializeField] private UETabEvent onDown;
	[SerializeField] private UETabEvent onTab;
	[SerializeField] private Image _image;
	[SerializeField] private Image _image2;
	[SerializeField] private Text _text;
	[SerializeField] private Text _text2;
	[SerializeField] private GameObject _dimmedObj;

	[SerializeField] private int index;

	private bool isSelected = false;
	private Vector3 startScale = Vector3.one;
	
	private void UpdateUI()
	{
		if(this.enabledObj) this.enabledObj.SetActive (this.isSelected);
		if(this.disabledObj) this.disabledObj.SetActive (!this.isSelected);
	}

}
