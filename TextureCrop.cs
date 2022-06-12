using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextureCrop : MonoBehaviour
{
    [Header("Texture")]
    public Texture2D m_Texture;

    [Header("Grid Size")]
    public int m_X_GridSize;
    public int m_Y_GridSize;

    [Space(5)]
    public GameObject m_SegmentImg_Prefab;
    public Transform m_Container;
    public RawImage m_TextureImg;


 
    public void CropTextureIntoSegments()
    {
        m_TextureImg.texture = m_Texture;

        int textureWidth = m_Texture.width;
        int textureHeight = m_Texture.height;


        int numberOfTextures = m_X_GridSize * m_Y_GridSize; // how many textures, like for 4x4 grid its 16,
        int segmentWidth = textureWidth / (m_X_GridSize); // width of one segment of texture, in 4x4 = 16 , 1 out of 16
        int segmentHeight = textureHeight / m_Y_GridSize;
        // ** These four arrays contains start and end points of each segment.

        int[] xStart = new int[numberOfTextures];
        int[] xEnd = new int[numberOfTextures];

        int[] yStart = new int[numberOfTextures];
        int[] yEnd = new int[numberOfTextures];

        // ** First we have to save the x,y - start,end points for each segmnet in above arrays
        // ** the temp variables are used to incremnet the value

        // ** e.g we have a  512 x 512 texture, and have pixels at points (x,y)
        // ** then x=0 index starts from left to right
        // ** and y=0 index starts from bottom to up

        // ** so xstart = 0 , and ystart = 512 
        // ** and xEnd = segment with e.g 100
        // ** and yEnd at Height-segmentwidth

        int xStartTemp = 0;
        int xEndTemp = segmentWidth;
        int yStartTemp = m_Texture.height;
        int yEndTemp = m_Texture.height - segmentHeight;

        int l_Counter = 1;

        for (int i = 0; i < numberOfTextures; i++)
        {
            xStart[i] = xStartTemp;
            xEnd[i] = xEndTemp;

            xStartTemp += segmentWidth;
            xEndTemp += segmentWidth;


            // ** After completion of on coloumn reset the xstart and xend 
            if (xStartTemp == m_Texture.width)
            {
                xStartTemp = 0;
            }

            if (xEndTemp > m_Texture.width)
            {
                xEndTemp = segmentWidth;
            }

            yStart[i] = yStartTemp;
            yEnd[i] = yEndTemp;

            l_Counter++;

            // ** After croping segments on One Horizontal changing the ystart and yend points
            if (l_Counter > m_X_GridSize)
            {
                yStartTemp -= segmentHeight;
                yEndTemp -= segmentHeight;
                l_Counter = 1;
            }
        }

       

        for (int k = 0; k < numberOfTextures; k++)
        {
            Texture2D texture = new Texture2D(segmentWidth, segmentHeight, TextureFormat.RGB24, false);

            // ** Get Data of start and end Points from arrays and draw the segment and save it

            for (int i = xStart[k]; i < xEnd[k]; i++)
            {
                for (int j = (yStart[k] - 1); j >= yEnd[k]; j--)
                {
                    texture.SetPixel(i, j, m_Texture.GetPixel(i, j));   
                }
            }

            SpawnSegmentOnUI(texture);

            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Application.dataPath + "/../SaveImages/";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllBytes(dirPath + (k + 1).ToString() + ".png", bytes);
        }
    }

    //** just ignore this part, spawing raw images on UI and applying Texture to it

    int xCounter = 0;
    int yCounter = 0;
    void SpawnSegmentOnUI(Texture2D texture)
    {
        int l_XS = -(m_X_GridSize * 60) / 2;
        int l_YS = -30 + (m_Y_GridSize * 60) / 2;

        GameObject l_Segment = (GameObject)Instantiate(m_SegmentImg_Prefab);
        l_Segment.transform.parent = m_Container;
        l_Segment.GetComponent<RectTransform>().localScale = Vector3.one;

        Vector3 l_Position = new Vector3(l_XS + xCounter * 60, l_YS - yCounter * 60, 0);
        l_Segment.GetComponent<RectTransform>().localPosition = l_Position;

        l_Segment.GetComponent<RawImage>().texture = texture;
        xCounter++;

        if (xCounter > (m_X_GridSize - 1))
        {
            xCounter = 0;
            yCounter++;
        }
    }
}
