using System;
//using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace PictureViewer
{
    public struct ImageData
    {
        public int id;
        public int detect_mode; // 0:off  1:on
        public DateTime t;
        public IplImage img;
        public bool ImgSaveFlag;

        // デフォルトコンストラクタ
        public ImageData(Int32 w, Int32 h)
        {
            id = 0;
            detect_mode = 0;
            t = DateTime.Now;
            img = new IplImage(w, h, BitDepth.U8, 1);
            ImgSaveFlag=false;
        }
/*        // コピーコンストラクタ
        public ImageData(ImageData src)
        {
            this.id = src.id;
            this.detect_mode = src.detect_mode;
            this.t = src.t;
            this.img = src.img.Clone();
        } */
    }
  /// <summary>
  /// 循環バッファ。
  /// </summary>
  /// <typeparam name="T">要素の型</typeparam>
    public class CircularBuffer : IEnumerable //(ImageData img) : IEnumerable(ImageData img)
    {
    #region フィールド

    ImageData[] data;
    IplImage[] img;
    int top, bottom;
    int mask;
    
    CvVideoWriter vw;
    int width, height;
    string savedir;

    #endregion
    #region 初期化

    public CircularBuffer() : this(256,640,480) {}

    /// <summary>
    /// 初期最大容量を指定して初期化。
    /// </summary>
    /// <param name="capacity">初期載大容量</param>
    public CircularBuffer(int capacity, int width, int height)
    {
      capacity = Pow2((uint)capacity);
      this.data = new ImageData[capacity];
      for (int i = 0; i < capacity; i++)
          this.data[i].img = new IplImage(width, height, BitDepth.U8, 1);
      this.img  = new IplImage[capacity];
      for (int i = 0; i < capacity; i++)
          this.img[i] = new IplImage(width, height, BitDepth.U8, 1);
      this.top = this.bottom = 0;
      this.mask = capacity - 1;
      this.width = width;
      this.height = height;
      this.savedir = @"C:\piccolo\";
    }

    static int Pow2(uint n)
    {
      --n;
      int p = 0;
      for (; n != 0; n >>= 1) p = (p << 1) + 1;
      return p + 1;
    }

    #endregion
    #region プロパティ

    /// <summary>
    /// 格納されている要素数。
    /// </summary>
    public int Count
    {
      get
      {
        int count = this.bottom - this.top;
        if (count < 0) count += this.data.Length;
        return count;
      }
    }

    /// <summary>
    /// i 番目の要素を読み書き。
    /// </summary>
    /// <param name="i">読み書き位置</param>
    /// <returns>読み出した要素</returns>
    public ImageData this[int i]
    {
      get { return this.data[(i + this.top) & this.mask]; }
      set { this.data[(i + this.top) & this.mask] = value;}
    }

    /// <summary>
    /// 末尾の要素を読み出し。
    /// </summary>
    /// <param name="elem">読み出した要素</param>
    public ImageData Last()
    {
        return this.data[(this.bottom) & this.mask];
    }

    /// <summary>
    /// 先頭の要素を読み出し。
    /// </summary>
    /// <param name="elem">読み出した要素</param>
    public ImageData First()
    {
        return this.data[(this.top) & this.mask];
    }

    /// <summary>
    /// i 番目の画像を読み書き。
    /// </summary>
    /// <param name="i">読み書き位置</param>
    /// <returns>読み出した要素</returns>
    public IplImage Image(int i)
    {
        return this.img[(i + this.top) & this.mask]; 
    }

    /// <summary>
    /// 末尾の画像を読み出し。
    /// </summary>
    /// <param name="elem">読み出した要素</param>
    public IplImage LastImage()
    {
        return this.img[(this.bottom) & this.mask];
    }

    /// <summary>
    /// 先頭の画像を読み出し。
    /// </summary>
    /// <param name="elem">読み出した要素</param>
    public IplImage FirstImage()
    {
         return this.img[(this.top) & this.mask];
    }
    #endregion
    #region 挿入・削除

    /// <summary>
    /// 配列を確保しなおす。
    /// </summary>
    /// <remarks>
    /// 配列長は2倍ずつ拡張していきます。
    /// </remarks>
    void Extend()
    {
        ImageData[] data = new ImageData[this.data.Length * 2];
      int i = 0;
      foreach (ImageData elem in this)
      {
        data[i] = elem;
        ++i;
      }
      this.top = 0;
      this.bottom = this.Count;
      this.data = data;
      this.mask = data.Length - 1;
    }

    /// <summary>
    /// i 番目の位置に新しい要素を追加。
    /// </summary>
    /// <param name="i">追加位置</param>
    /// <param name="elem">追加する要素</param>
    public void Insert(int i, ImageData elem)
    {
      if (this.Count >= this.data.Length - 1)
        this.Extend();

      if (i < this.Count / 2)
      {
        for (int n = 0; n <= i; ++n)
        {
          this[n - 1] = this[n];
        }
        this.top = (this.top - 1) & this.mask;
        this[i] = elem;
      }
      else
      {
        for (int n = this.Count; n > i; --n)
        {
          this[n] = this[n - 1];
        }
        this[i] = elem;
        this.bottom = (this.bottom + 1) & this.mask;
      }
    }

    /// <summary>
    /// 先頭に新しい要素を追加。
    /// </summary>
    /// <param name="elem">追加する要素</param>
    public void InsertFirst(ImageData elem)
    {
      if (this.Count >= this.data.Length - 1)
        this.Extend();

      this.top = (this.top - 1) & this.mask;
      this.data[this.top] = elem;
      Cv.Copy(elem.img,this.img[this.top]);
    }

    /// <summary>
    /// 末尾に新しい要素を追加。
    /// </summary>
    /// <param name="elem">追加する要素</param>
    public void InsertLast(ImageData elem)
    {
      if (this.Count >= this.data.Length - 1)
        this.Extend();

      this.data[this.bottom] = elem;
      this.bottom = (this.bottom + 1) & this.mask;
      Cv.Copy(elem.img, this.img[this.bottom]);
    }

    /// <summary>
    /// i 番目の要素を削除。
    /// </summary>
    /// <param name="i">削除位置</param>
    public void Erase(int i)
    {
      for (int n = i; n < this.Count - 1; ++n)
      {
        this[n] = this[n + 1];
        Cv.Copy(this.img[n+1], this.img[n]);
      }
      this.bottom = (this.bottom - 1) & this.mask;
    }

    /// <summary>
    /// 先頭の要素を削除。
    /// </summary>
    public void EraseFirst()
    {
      this.top = (this.top + 1) & this.mask;
    }

    /// <summary>
    /// 末尾の要素を削除。
    /// </summary>
    public void EraseLast()
    {
        // 初期化チェック
        if (this.data[this.bottom].ImgSaveFlag==false && this.data[(this.bottom - 1) & this.mask].ImgSaveFlag==true)
        {
            string fn = savedir + this.data[this.bottom].t.ToString("yyyyMMdd") + @"\";
            // フォルダ (ディレクトリ) が存在しているかどうか確認する
            if (!System.IO.Directory.Exists(fn))
            {
                System.IO.Directory.CreateDirectory(fn);
            }
            int NoCapDev = 2;
            fn += string.Format("{00}_", NoCapDev) + this.data[this.bottom].t.ToString("yyyyMMdd_HHmmss_fff") + ".avi";
            VideoWriterInit(fn);
        }
        // 書き込みチェック
        if (this.data[this.bottom].ImgSaveFlag)
        {
            VideoWriterFrame();
        }
        // 開放チェック
        if (this.data[this.bottom].ImgSaveFlag == true && this.data[(this.bottom - 1) & this.mask].ImgSaveFlag == false)
        {
            VideoWriterRelease();
        }
        // 更新
        this.bottom = (this.bottom - 1) & this.mask;
    }

    #endregion
    #region IEnumerable<T> メンバ

    public IEnumerator GetEnumerator()
    {
      if (this.top <= this.bottom)
      {
        for (int i = this.top; i < this.bottom; ++i)
          yield return this.data[i];
      }
      else
      {
        for (int i = this.top; i < this.data.Length; ++i)
          yield return this.data[i];
        for (int i = 0; i < this.bottom; ++i)
          yield return this.data[i];
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
    #region VideoWriter

    /// <summary>
    /// 画像を保存します。
    /// </summary>
    /// <remarks>
    /// ビデオ書き込み初期化
    /// </remarks>
    public void VideoWriterInit(string fn)
    {
        int codec = Cv.FOURCC('D', 'I', 'B', ' ');  // 0; //非圧縮avi
        this.vw = new CvVideoWriter(fn, codec, 29.97, new CvSize(this.width, this.height),false);
    }

    /// <summary>
    /// 画像を保存します。
    /// </summary>
    /// <remarks>
    /// ビデオ書き込み
    /// </remarks>
    public void VideoWriterFrame()
    {
        // 文字入れ

        vw.WriteFrame(this.img[this.bottom]);
    }

    /// <summary>
    /// 画像を保存します。
    /// </summary>
    /// <remarks>
    /// ビデオ書き込み開放処理
    /// </remarks>
    public void VideoWriterRelease()
    {
        vw.Dispose();
    }

    /// <summary>
    /// 画像を保存します。
    /// </summary>
    /// <remarks>
    /// Set save dir name
    /// </remarks>
    public void VideoWriterSetSaveDir(string s)
    {
        
    }
    #endregion

  }
}
