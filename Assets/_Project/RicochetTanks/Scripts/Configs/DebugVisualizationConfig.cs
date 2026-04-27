using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Debug Visualization Config", fileName = "DebugVisualizationConfig")]
    public sealed class DebugVisualizationConfig : ScriptableObject
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private bool _drawLabels = true;
        [SerializeField] private bool _drawPredictedNextSegment = true;
        [SerializeField] private float _predictedSegmentLength = 3f;

        public bool IsEnabled => _isEnabled;
        public bool DrawLabels => _drawLabels;
        public bool DrawPredictedNextSegment => _drawPredictedNextSegment;
        public float PredictedSegmentLength => _predictedSegmentLength;
    }
}
