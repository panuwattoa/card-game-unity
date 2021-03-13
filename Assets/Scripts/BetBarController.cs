using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BetBarController : MonoBehaviour, IDragHandler, IPointerDownHandler
{
	[SerializeField] private BetController m_betController;
	private int xPosition;
	private const int m_positonMix = -148;
	private const int m_positionMax = 119;
	public void OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log("OnPointerDown + " + eventData.position);
		var rect1 = GetComponent<RectTransform>();

		Vector2 localCursor;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
			return;

		int xpos = (int)(localCursor.y);

		if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
		else xpos += (int)rect1.rect.width / 2;

		xPosition = xpos;

		OnValueChange();
	}

	public void OnDrag(PointerEventData eventData)
	{
		var rect1 = (RectTransform)this.gameObject.transform;

		Vector2 localCursor;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
			return;

		int xpos = (int)(localCursor.y);

		if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
		else xpos += (int)rect1.rect.width / 2;

		xPosition = xpos;
		OnValueChange();
	}

	private void OnValueChange()
	{
		if (xPosition < m_positonMix)
			xPosition = m_positonMix;

		if (xPosition > m_positionMax)
			xPosition = m_positionMax;


		float v = xPosition - m_positonMix;

		m_betController.OnChangeBetValue(v / (m_positionMax - m_positonMix));
	}

}
