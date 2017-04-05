using Leap;
using UnityEngine;
using System.Collections;

public class LeapListener
{
	//Leap controller.
	private Controller controller = null;

	//Minimum distance from hand for thumb to be recognized.
	public static float thumbDistance = 40;

	//Current frame.
	public Frame frame = null;

	//Current hand
	public Hand hand;

	//Hands contained in the current frame.
	public int hands = 0;

	//Fingers contained in the current hand.
	public int fingers = 0;
	 

	//Various easy-access hand values.
	public float handPitch = 0.0F;
	public float handRoll = 0.0F;
	public float handYaw = 0.0F;

	//Hand and finger positions.
	public Vector handPosition = Vector.Zero;
	public Vector fingerPosition = Vector.Zero;
	public Vector handDirection = Vector.Zero;
	public Vector fingerDirection = Vector.Zero;

	//Quick-find for the right thumb.
	public Finger thumb = null;

	//Timestamp of the current frame.
	public long timestamp = 0;

	//Is the Leap connected?
	public static bool connected = false;

	public InteractionBox iBox;

	//Function to map leap coordinates to a 3D coordinate system
	public Vector leapToWorld(Leap.Vector leapPoint, InteractionBox iBox)
	{
		leapPoint.x *= -1.0f;
		leapPoint.z *= -1.0f; //right-hand to left-hand rule
		Leap.Vector normalized = iBox.NormalizePoint(leapPoint, false);
		normalized += new Leap.Vector(0.0f, 0f, 0.0f); //recenter origin
		return normalized * 1.0f; //scale
	}

	//Member Function: refresh
	public bool refresh() 
	{
		//Try.
		try
		{
			//If there's no controller, make a new one.
			if (controller == null) 
			{
				controller = new Controller();
			}

			//Check if the controller is connected.
			connected = controller.IsConnected; //&& controller.Devices.Count > 0; // && controller.Devices[0].IsValid;

			//If we're connected, update.
			if (connected)
			{
				//Get the most recent frame.
				frame = controller.Frame();

				//Get the interaction box
				iBox = frame.InteractionBox;

				//Get the hands count
				hands = frame.Hands.Count;

				//Get the current timestep
				timestamp = frame.Timestamp;

				//If we see some hands, get their positions and their fingers.
				if (frame.Hands.Count > 0)
				{
					//Get the hand's position, size, and first finger.
					hand = frame.Hands [0];
					fingers = hand.Fingers.Count;

					handPosition = hand.PalmPosition;
					handDirection = hand.Direction;
					fingerDirection = hand.Direction;

					//Get the hand's normal vector and direction.
					Vector normal = hand.PalmNormal;
					Vector direction = hand.Direction;

					//Get the hand's angles.
					handPitch = (float) direction.Pitch * 180.0f / (float) System.Math.PI;
					handRoll = (float) normal.Roll * 180.0f / (float) System.Math.PI;
					handYaw = (float) direction.Yaw * 180.0f / (float) System.Math.PI;

					//fingerPosition = hand.TipPosition;

					/*thumb = null;

					//Find the thumb for the primary hand.
					foreach (Leap.Finger finger in hand.Fingers)
					{ 
						if (thumb != null && finger.TipPosition.x < thumb.TipPosition.x && finger.TipPosition.x < handPosition.x)
							thumb = finger;

						else if (thumb == null && finger.TipPosition.x < handPosition.x - thumbDistance)
							thumb = finger;
					}*/
				}
			}

			//Otherwise, reset all outgoing data to 0.
			else
			{
				//Fingers contained in the current frame.
				fingers = 0;

				//Hands contained in the current frame.
				hands = 0;

				//Various easy-access hand values.
				handPitch = 0.0F;
				handRoll = 0.0F;
				handYaw = 0.0F;

				//Hand and finger positions.
				handPosition = Vector.Zero;
				fingerPosition = Vector.Zero;
				handDirection = Vector.Zero;
				fingerDirection = Vector.Zero;

				//Quick-find for the right thumb.
				//thumb = null;
			}

			return true;
		}

		//In the event that anything goes wrong while reading and converting tracking data, log the exception.
		catch (System.Exception e) { UnityEngine.Debug.LogException(e);  return false; }
	}

	//Member Function: rotation
	public UnityEngine.Vector3 rotation (Leap.Hand hand)
	{
		//Create a new vector for our angles.
		UnityEngine.Vector3 rotationAngles = new UnityEngine.Vector3(0, 0, 0);

		//Get the hand's normal vector and direction.
		Vector normal = hand.PalmNormal;
		Vector direction = hand.Direction;

		//Set the values.
		rotationAngles.x = (float) direction.Pitch * 180.0f / (float) System.Math.PI;
		rotationAngles.z = (float) normal.Roll * 180.0f / (float) System.Math.PI;
		rotationAngles.y = (float) direction.Yaw * 180.0f / (float) System.Math.PI;

		//Return the angles.
		return rotationAngles;
	}

}