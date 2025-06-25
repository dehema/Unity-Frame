using System.Collections.Generic;

namespace Rain.Core
{
	public static class SequenceManager
	{
		private static Stack<Sequence> sequencePool = new Stack<Sequence>();
		
		public static Sequence GetSequence()
		{
			if (sequencePool.Count <= 0)
			{
				return ProcessSequence(new Sequence(), false);
			}

			return ProcessSequence(sequencePool.Pop());
		}

		private static Sequence ProcessSequence(Sequence sequence, bool reset = true)
		{
			if (reset)
			{
				sequence.Reset();
			}

			sequence.Recycle = sequence.KillSequence;
			F8Tween.Instance.OnUpdateAction += sequence.Update;
			return sequence;
		}

		public static void KillSequence(this Sequence sequence)
		{
			F8Tween.Instance.OnUpdateAction -= sequence.Update;
			sequence.Reset();
			sequencePool.Push(sequence);
		}
	}

}

