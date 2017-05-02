using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace carGame1
{
    public class Physics
    {
        public Rectangle boxCollisionRectangle(Rectangle a, Rectangle b)
        {
            int top, bottom, left, right;
            top = Math.Max(a.Top, b.Top);
            bottom = Math.Min(a.Bottom, b.Bottom);
            left = Math.Max(a.Left, b.Left);
            right = Math.Min(a.Right, b.Right);
            return new Rectangle(left, top, (right - left), (bottom - top));
        }
        public Vector2 pixelCollisionPoint(Color[] colorData1, Color[] colorData2, Rectangle a, Rectangle b)
        {

            Rectangle intersection = boxCollisionRectangle(a, b);
            Vector2 point = new Vector2(-1, -1);
            if (colorData1 == null || colorData2 == null)
                return point;
            for (int y = intersection.Top; y < intersection.Bottom; y++)
            {
                for (int x = intersection.Left; x < intersection.Right; x++)
                {
                    Color A = colorData1[(y - a.Top) * a.Width + (x - a.Left)];
                    Color B = colorData2[(y - b.Top) * b.Width + (x - b.Left)];
                    if (A.A != 0 && B.A != 0)
                    {
                        point = new Vector2(x, y);
                        return point;
                    }
                }
            }
            return point;
        }
        public bool TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        public Vector2 pixelCollisionPoint(Texture2D sprite1, Texture2D sprite2, Rectangle a, Rectangle b)
        {
            Color[] colorData1 = new Color[sprite1.Width * sprite1.Height];
            Color[] colorData2 = new Color[sprite2.Width * sprite2.Height];
            sprite1.GetData<Color>(colorData1);
            sprite2.GetData<Color>(colorData2);
            Vector2 point = new Vector2(-1, -1);
            Rectangle intersection = boxCollisionRectangle(a, b);

            for (int y = intersection.Top; y < intersection.Bottom; y++)
            {
                for (int x = intersection.Left; x < intersection.Right; x++)
                {
                    Color A = colorData1[(y - a.Top) * a.Width + (x - a.Left)];
                    Color B = colorData2[(y - b.Top) * b.Width + (x - b.Left)];
                    if (A.A != 0 && B.A != 0)
                    {
                        point = new Vector2(x, y);
                        return point;
                    }
                }
            }
            return point;
        }
        public static bool IntersectPixels(
            Matrix transformA, int widthA, int heightA, Color[] dataA,
            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
        
        
    }
}