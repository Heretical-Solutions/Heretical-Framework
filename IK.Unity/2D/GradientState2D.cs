namespace HereticalSolutions.IK.Unity
{
	public class GradientState2D
	{
		public float previousGradient;
		
		public float momentum;
		
		public GradientState2D()
		{
			previousGradient = 0f;

			momentum = 0f;
		}
	}
}