namespace Laboratory_Work_One
{
    class ShapeParams
    {
        public ShapeParams(int width, int height)
        {
            Top = height;
            Bottom = 0;
            Left = width;
            Right = 0;
        }

        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public int Right { get; private set; }
        public int Left { get; private set; }

        public void SetShapeParams(int byWidth, int byHeight)
        {
            if (byWidth > Right)
            {
                Right = byWidth;
            }
            
            if (Left > byWidth)
            {
                Left = byWidth;
            }

            if (byHeight > Bottom)
            {
                Bottom = byHeight;
            }

            if (Top > byHeight)
            {
                Top = byHeight;
            }
        }
    }
}
