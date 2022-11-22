using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TeleportAimLaserVisualBezier : TeleportSupport
{
	/// <summary>
	/// This prefab will be instantiated when the aim visual is awakened, and will be set active when the 
	/// user is aiming, and deactivated when they are done aiming.
	/// </summary>
	[Tooltip("This prefab will be instantiated when the aim visual is awakened, and will be set active when the user is aiming, and deactivated when they are done aiming.")]
	public LineRenderer LaserPrefab;
    
    [SerializeField, Range(3, 100)] private int m_Resolution = 500;

	private readonly Action _enterAimStateAction;
	private readonly Action _exitAimStateAction;
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;
	private LineRenderer _lineRenderer;
	private Vector3[] _linePoints;

	public TeleportAimLaserVisualBezier()
	{
		_enterAimStateAction = EnterAimState;
		_exitAimStateAction = ExitAimState;
		_updateAimDataAction = UpdateAimData;
	}

	private void EnterAimState()
	{
		_lineRenderer.gameObject.SetActive(true);
	}

	private void ExitAimState()
	{
		_lineRenderer.gameObject.SetActive(false);
	}

	void Awake()
	{
		LaserPrefab.gameObject.SetActive(false);
		_lineRenderer = Instantiate(LaserPrefab);
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		LocomotionTeleport.EnterStateAim += _enterAimStateAction;
		LocomotionTeleport.ExitStateAim += _exitAimStateAction;
		LocomotionTeleport.UpdateAimData += _updateAimDataAction;
	}

	/// <summary>
	/// Derived classes that need to use event handlers need to override this method and
	/// call the base class to ensure all event handlers are removed as intended.
	/// </summary>
	protected override void RemoveEventHandlers()
	{
		LocomotionTeleport.EnterStateAim -= _enterAimStateAction;
		LocomotionTeleport.ExitStateAim -= _exitAimStateAction;
		LocomotionTeleport.UpdateAimData -= _updateAimDataAction;
		base.RemoveEventHandlers();
	}

	private void UpdateAimData(LocomotionTeleport.AimData obj) {
        _lineRenderer.sharedMaterial.color = Color.green;

        //Note: The line renderer is dis-/enabled in the Enter/ExitAimState methods.
        //   This dis-/enables it based on whether the user is hitting a destination.
        if (LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim || !obj.TargetValid) {
            _lineRenderer.gameObject.SetActive(false);
            return;
        }
        _lineRenderer.gameObject.SetActive(true);

		var points = obj.Points;
        _lineRenderer.positionCount = m_Resolution;
        var firstElement = points[0];
        var lastElement = points[^1];
        LocomotionTeleport.InputHandler.GetAimData(out Ray aimRay);
        Vector3 inBetween = (lastElement - firstElement) / 2;

        for (int i = 0; i < m_Resolution; i++)
		{
            float t = i / (float)(m_Resolution - 1);
            _lineRenderer.SetPosition(i, GetPoint(firstElement, firstElement + aimRay.direction * inBetween.magnitude, lastElement, t));
		}
	}

    private Vector3 GetPoint(Vector3 P0, Vector3 P1, Vector3 P2, float t) {
        return Vector3.Lerp(
            Vector3.Lerp(P0, P1, t),
            Vector3.Lerp(P1, P2, t),
            t
        );
    }
}
