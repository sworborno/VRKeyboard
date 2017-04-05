using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap;

using System.IO;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra.Factorization;


//using MathNet.Numerics.Data.Text;

class FilterData : MonoBehaviour 
{
	//Leap listener.
	private LeapListener listener;

	//Leap box.
	private InteractionBox normalizedBox;

	public GameObject KeyboardSurface = null;

	String filePath = "C:\\Users\\Jiban Adhikary\\Documents\\HandCollision\\data2.tsv";
	String fileTobeFitted = "C:\\Users\\Jiban Adhikary\\Documents\\HandCollision\\Assets\\position1.txt";

	//Distance modifiers.
	public float depth = 20.0F;
	public float verticalOffset = -20.0F;

	public List<Vector3> linePoints;
	public List<Vector3> avgLinePoints3d;
	public List<Vector3> avgLinePoints2d;

	public int countAvg = 0;
	public int avgWindow = 5;
	public int count = 0;

	public GameObject testObject = null;

	Hand hand = null;
	Finger thumb = null;
	Finger index = null;
	Finger middle = null;
	Finger ring = null;
	Finger pinky = null;


	Leap.Vector currPoint = new Leap.Vector(0, 0, 0);
	Leap.Vector normalizedPosition = new Leap.Vector(0, 0, 0);

	bool runFile = false; 

	void movingAvg(int index)
	{
		float sumX = 0;
		float sumY = 0;
		float sumZ = 0;
		for (int i = index-1; i >= index-avgWindow; i--) {
			sumX += linePoints [i].x;
			sumY += linePoints [i].y;
			sumZ += linePoints [i].z;
		}

		float avgX = sumX / (avgWindow-1);
		float avgY = sumY / (avgWindow-1);
		float avgZ = sumZ / (avgWindow-1);
		avgLinePoints3d.Add (new Vector3(avgX, avgY, avgZ));
		avgLinePoints2d.Add (new Vector2(avgX, avgZ));



		//Append average x and z coordinates in a tsv file
		//System.IO.File.AppendAllText(filePath, ""+avgX+"\t"+avgZ + System.Environment.NewLine);

		//Apend average x, y and z coordinates in a simple text file/ csv file
		//System.IO.File.AppendAllText(filePath, ""+avgX+","+avgZ + System.Environment.NewLine);

		//Append average x, y and z coordinates in the file
		//System.IO.File.AppendAllText(filePath, ""+avgX+"\t"+avgY+"\t"+avgZ +System.Environment.NewLine);

		//Append average x, y and z coordinates in the csv file
		//System.IO.File.AppendAllText(filePath, ""+avgX+","+avgY+","+avgZ +System.Environment.NewLine);

		//Debug.Log ("Average values: "+ avgX + "," + avgY + "," + avgZ);
		countAvg++;
	}

	//OnEnable.
	public void Start() 
	{
		//listener = new LeapListener();

		if (runFile == false) 
		{
			planeFitting (fileTobeFitted);
			runFile = true;
		}
			

	}

	public void OnEnable()
	{
		listener = new LeapListener();
	}


	//Update.
	public void Update()
	{   
		if (listener == null) 
			listener = new LeapListener();

		//Update the listener.
		listener.refresh();

		Debug.Log ("Frame id: " + listener.frame.Id + ", timestamp: " + listener.frame.Timestamp + ", hands: " + listener.hands);

		//Get a normalized box.
		normalizedBox = listener.iBox;  //accessing interaction box in the listener class

		//planeFitting (fileTobeFitted);


		//First, get any hands that are present.
		if (listener.hands > 0)
		{
			hand = listener.hand;
			index = hand.Fingers [(int)Finger.FingerType.TYPE_INDEX];
			thumb = hand.Fingers [(int)Finger.FingerType.TYPE_THUMB];
			middle = hand.Fingers [(int)Finger.FingerType.TYPE_MIDDLE];
			ring = hand.Fingers [(int)Finger.FingerType.TYPE_RING];
			pinky = hand.Fingers [(int)Finger.FingerType.TYPE_PINKY];

			if (index.IsExtended && !middle.IsExtended && !ring.IsExtended && !pinky.IsExtended)
			{
				if (count >= avgWindow)
					movingAvg (count);
				currPoint = index.TipPosition;
				normalizedPosition = listener.leapToWorld (currPoint, normalizedBox);
				linePoints.Add (new Vector3(normalizedPosition.x, normalizedPosition.y,normalizedPosition.z));



				//Camera camera = this.transform.Find("CenterEyeAnchor").GetComponent<Camera>();

				//Vector3 screenPosition = camera.WorldToScreenPoint (new Vector3 (currPoint.x, currPoint.y, currPoint.z));
				//Vector3 screenPosition = camera.WorldToScreenPoint (new Vector3 (normalizedPosition.x, normalizedPosition.y, normalizedPosition.z));



				Debug.Log ("Current Tip Position: "+currPoint);
				Debug.Log ("Current Normalized Tip Position: "+normalizedPosition);
				//Debug.Log ("Screen Position: "+camera.WorldToScreenPoint(new Vector3(currPoint.x, currPoint.y, currPoint.z)));
				//Debug.Log ("Position of LMHrig: "+this.transform.Find ("LMHeadMountedRig").position);

				/*testObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				testObject.AddComponent<Rigidbody> ();
				testObject.transform.position = new Vector3 (normalizedPosition.x, normalizedPosition.y, normalizedPosition.z);
				//testObject.transform.position = new Vector3(screenPosition.x, screenPosition.y, screenPosition.z);
				//testObject.transform.position = new Vector3(currPoint.x, currPoint.y, currPoint.z);
				testObject.transform.localScale = new Vector3 (0.03F, 0.03F, 0.03F);*/
				//Instantiate (testObject, new Vector3 (normalizedPosition.x, normalizedPosition.y,normalizedPosition.z), Quaternion.Euler (0, 0, 0));

				/*if (count == 100) 
				{
					GameObject testObject2 = GameObject.CreatePrimitive (PrimitiveType.Plane);
					testObject2.AddComponent<Rigidbody> ().isKinematic = true;



					//testObject.transform.position = new Vector3 (normalizedPosition.x, normalizedPosition.y, normalizedPosition.z);
					testObject2.transform.position = new Vector3(0f, 0f, camera.transform.transform.position.z+(-0.2f));
					testObject2.transform.localScale = new Vector3 (1, 0.1F, 0.1F);
					testObject2.transform.rotation = Quaternion.Euler(95, 0, 0);
				}*/



				/*GameObject keysurf = null;

				try{
					keysurf = GameObject.Find ("KeyboardSurface");
					Debug.Log ("Position of keysurf: "+keysurf.transform.position);
				}
				catch(Exception e)
				{
					Debug.LogException (e, this);
				}*/

				count++;	
			}


			//Debug.Log ("Index finger direction: "+finger.Direction);

		}
			


	}

	public void planeFitting(String inputFile)
	{
		int rowCounter = 0;
		int lineCounter = 0;
		String line;
		StreamReader reader;


		// Initialize a dense matrix of size 1000 x 3
		DenseMatrix M = new DenseMatrix(1000, 3);



		//Read the file as a streamreader
		reader = new StreamReader(inputFile, Encoding.Default); 

		//get the first line of the streamreader
		line = reader.ReadLine(); 


		while (line != null) 
		{
			//Place each comma seperated values in an array of string
			string[] rows = line.Split (',');  

			// Convert each string to its corresponding double value
			double x = Convert.ToDouble (rows [0]);  
			double y = Convert.ToDouble (rows [1]);
			double z = Convert.ToDouble (rows [2]);

			// Initialize a dense vector of size 3
			MathNet.Numerics.LinearAlgebra.Double.Vector rowData = new DenseVector (3); 

			// Assign converted values to dense vector
			rowData [0] = x; 
			rowData [1] = y;
			rowData [2] = z;

			// Insert the dense vector at specific positions of the matrix
			//This is the initial data matrix
			M.SetRow (rowCounter, rowData);


			//Debug.Log(M.At(rowCounter, 0)+","+M.At(rowCounter, 1)+","+ M.At(rowCounter, 2));

			//Increment the rowCounter 
			rowCounter++;

			// Read the next line from the streamreader
			line = reader.ReadLine ();
		}


		DenseMatrix DataMatrix = new DenseMatrix(rowCounter, 3);

		for (int i = 0; i < rowCounter; i++) 
		{
			DataMatrix.SetRow (i, M.Row (i));
		}

		//Debug.Log (DataMatrix.RowCount + "");

		// Find the centroid of the matrix M
		DenseMatrix centroid = new DenseMatrix(rowCounter, 3);

		Double c0 = MathNet.Numerics.Statistics.Statistics.Mean(DataMatrix.Column(0));
		Double c1 = MathNet.Numerics.Statistics.Statistics.Mean(DataMatrix.Column(1));
		Double c2 = MathNet.Numerics.Statistics.Statistics.Mean(DataMatrix.Column(2));

		MathNet.Numerics.LinearAlgebra.Double.Vector centroidRow = new DenseVector(3);
		centroidRow [0] = c0; //(c0*1000)/rowCounter;
		centroidRow [1] = c1; //(c1*1000)/rowCounter;
		centroidRow [2] = c2; //(c2*1000)/rowCounter;

		for(int j = 0; j < rowCounter; j++)
			centroid.SetRow(j, centroidRow);

		//Debug.Log(centroid.At(0, 0)+","+centroid.At(0, 1)+","+ centroid.At(0, 2)+"****************************");


		DenseMatrix DataMatrixNorm = new DenseMatrix(rowCounter, 3);


		//Subtract centroid from DataMatrix
		DataMatrixNorm = DataMatrix - centroid;

		//for(int j = 0; j < rowCounter; j++)
			//Debug.Log(DataMatrixNorm.At(j,0)+","+DataMatrixNorm.At(j,1)+","+DataMatrixNorm.At(j,2)+"");

		Svd <double> DataSVD = DataMatrixNorm.Svd(true); 
		//Matrix <Double> DataSVD = DataMatrixNorm.Svd(true); 

		Matrix <Double> U = DataSVD.U;  // Left singular vectors n x n unitary matrix 
		Matrix <Double> W = DataSVD.W;  // Singular values as a diagonal matrix
		Matrix <Double> VT = DataSVD.VT; //Transpose of V, right singular vectors

		//Debug.Log (W.RowCount+","+W.ColumnCount+"*************************");
		Matrix <Double> V = VT.Transpose();


		MathNet.Numerics.LinearAlgebra.Vector <Double> N = (-1 / V.At (V.RowCount - 1, V.ColumnCount - 1))*V.Column(V.ColumnCount-1); 

		//Debug.Log ("Size of N: "+ N.Count+"");

		Double A = N.At (0);
		Double B = N.At (1);
		Double C = -(centroidRow.At(0) * N.At(0) + centroidRow.At(1) * N.At(1) + centroidRow.At(2) * N.At(2)); 


		// Debug.Log ("Value of C: "+ C +"---");

		int intPoints = 20;
		MathNet.Numerics.LinearAlgebra.Vector <Double> X = new DenseVector(intPoints); 


		MathNet.Numerics.LinearAlgebra.Vector <Double> xValues = DataMatrixNorm.Column(0);
		MathNet.Numerics.LinearAlgebra.Vector <Double> yValues = DataMatrixNorm.Column(1);
		MathNet.Numerics.LinearAlgebra.Vector <Double> zValues = DataMatrixNorm.Column(2);

		Double spacing = (xValues.Maximum() - xValues.Minimum())/ (intPoints -1);

		if (spacing < 0)
			spacing *= -1; 

		for (double i = xValues.Minimum(); i < xValues.Maximum(); i += spacing) 
		{
			//X.Add (i);
		}


	}


	public void CollisionDetected(CollisionDetection collisionScript, Collider surfCollider, Collider fingerCollider, Vector3 point)
	{
		//Debug.Log("child collided****************************");
		Camera camera = this.transform.Find("CenterEyeAnchor").GetComponent<Camera>();

		Vector3 pointScreenpos = camera.WorldToScreenPoint(point);
		float tipx = hand.Fingers [(int)Finger.FingerType.TYPE_INDEX].TipPosition.x;
		float tipy = hand.Fingers [(int)Finger.FingerType.TYPE_INDEX].TipPosition.y;
		float tipz = hand.Fingers [(int)Finger.FingerType.TYPE_INDEX].TipPosition.z;
		Vector3 tipPosition = new Vector3 (tipx,tipy,tipz);

		Vector3 w2sposition = camera.WorldToScreenPoint (tipPosition);
		Debug.Log (fingerCollider.name + " " + camera.WorldToScreenPoint (fingerCollider.transform.position) + " " + "collides with " + surfCollider.name + " at position " + pointScreenpos+"*********************");//
		//+ " & Current Tip Position: "+w2sposition);
	}
}