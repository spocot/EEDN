namespace EEDN.Render
{
    public class Matrix4f
    {
        public float[][] Matrix =
        {
            new float[4],
            new float[4],
            new float[4],
            new float[4]
        };

        public Matrix4f InitTranslation(float x, float y, float z)
        {
            Matrix[0][0] = 1; Matrix[0][1] = 0; Matrix[0][2] = 0; Matrix[0][3] = x;
            Matrix[1][0] = 0; Matrix[1][1] = 1; Matrix[1][2] = 0; Matrix[1][3] = y;
            Matrix[2][0] = 0; Matrix[2][1] = 0; Matrix[2][2] = 1; Matrix[2][3] = z;
            Matrix[3][0] = 0; Matrix[3][1] = 0; Matrix[3][2] = 0; Matrix[3][3] = 1;

            return this;
        }

        public Matrix4f Mul(Matrix4f r)
        {
            Matrix4f re = new Matrix4f();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    re[i, j] =
                        Matrix[i][0] * r[0, j] +
                        Matrix[i][1] * r[1, j] +
                        Matrix[i][2] * r[2, j] +
                        Matrix[i][3] * r[3, j];
                }
            }

            return re;
        }

        public static Matrix4f operator *(Matrix4f c1, Matrix4f c2)
        {
            return c1.Mul(c2);
        }

        public Matrix4f InitScale(float x, float y, float z)
        {
            Matrix[0][0] = x; Matrix[0][1] = 0; Matrix[0][2] = 0; Matrix[0][3] = 0;
            Matrix[1][0] = 0; Matrix[1][1] = y; Matrix[1][2] = 0; Matrix[1][3] = 0;
            Matrix[2][0] = 0; Matrix[2][1] = 0; Matrix[2][2] = z; Matrix[2][3] = 0;
            Matrix[3][0] = 0; Matrix[3][1] = 0; Matrix[3][2] = 0; Matrix[3][3] = 1;

            return this;
        }

        public float this[int x, int y]
        {
            get => Matrix[x][y];
            set => Matrix[x][y] = value;
        }

        public Matrix4f InitIdentity()
        {
            Matrix[0][0] = 1; Matrix[0][1] = 0; Matrix[0][2] = 0; Matrix[0][3] = 0;
            Matrix[1][0] = 0; Matrix[1][1] = 1; Matrix[1][2] = 0; Matrix[1][3] = 0;
            Matrix[2][0] = 0; Matrix[2][1] = 0; Matrix[2][2] = 1; Matrix[2][3] = 0;
            Matrix[3][0] = 0; Matrix[3][1] = 0; Matrix[3][2] = 0; Matrix[3][3] = 1;

            return this;
        }

        public Matrix4f InitOrthographic(float left, float right, float bottom, float top, float near, float far)
        {
            float width = right - left;
            float height = top - bottom;
            float depth = far - near;

            Matrix[0][0] = 2 / width; Matrix[0][1] = 0; Matrix[0][2] = 0; Matrix[0][3] = -(right + left) / width;
            Matrix[1][0] = 0; Matrix[1][1] = 2 / height; Matrix[1][2] = 0; Matrix[1][3] = -(top + bottom) / height;
            Matrix[2][0] = 0; Matrix[2][1] = 0; Matrix[2][2] = -2 / depth; Matrix[2][3] = -(far + near) / depth;
            Matrix[3][0] = 0; Matrix[3][1] = 0; Matrix[3][2] = 0; Matrix[3][3] = 1;

            return this;
        }
    }
}
