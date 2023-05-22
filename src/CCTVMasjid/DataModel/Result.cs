namespace CCTVMasjid.DataModel
{
    public class Result
    {
        /// <summary>
        /// x1, y1, x2, y2 in page coordinates.
        /// </summary>
        public float[] BoundingBox { get; }

        /// <summary>
        /// The Bounding box category.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Confidence level.
        /// </summary>
        public float Confidence { get; }

        public Result(float[] boundingBox, string label, float confidence)
        {
            BoundingBox = boundingBox;
            Label = label;
            Confidence = confidence;
        }
    }
}
