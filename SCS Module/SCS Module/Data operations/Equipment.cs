using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;

namespace SCS_Module
{
    public class Equipment
    {
        //первая стадия
        public int id;                                         //номера карточек
        public string name;                                     //имена
        public string description;                             //описания
        public string preview;                                  //Превью каждого оборудования

        //вторая стадия
        public Dictionary<string, string> properties = new Dictionary<string, string>();    //свойства одного оборудования
        public List<Compatibility> compatibilities = new List<Compatibility>();             //совместимости интерфейсов одного оборудования
        public List<string> PICS = new List<string>();                                      //фотки оборудований

        //третья стадия
        public VectorPic inPlacementScheme;                                              //схема расположения
        public VectorPic inBox;                                                         //схема шкафа
        public VectorPic inConnectionScheme;                                           //схема соединения
        public VectorPic inStructural;                                                    //структурная схема 
        public byte[] bytedFile = null;                                                     //семейство

        public bool isWire = false;
        public bool isBox = false;
        public bool isInBox = false;


        //дополнительные поля
        public int familyID = -1;

        public int inPlacementSchemeID = -1;                                              //схема расположения
        public int inBoxID = -1;                                                         //схема шкафа
        public int inConnectionSchemeID = -1;                                            //схема соединения
        public int inStructuralID = -1;

        public int photosID = -1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">id(int),name(string),desc(string),prev(BASE64),prop(Dictionary<string, string>),comp(List<compatibility>),pics(List<BASE64>),place(BASE64),box(BASE64),conn(BASE64),struct(BASE64),file(byte[]),wire(bool)</param>
        /// <param name="full"></param>
        public Equipment(SqlDataReader reader, Stage stage)
        {
            try
            {
                id = Convert.ToInt32(reader[0].ToString());
                name = reader[1].ToString();
                description = reader[2].ToString();
                preview = reader[3].ToString();

                properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader[4].ToString());
                compatibilities = JsonConvert.DeserializeObject<List<Compatibility>>(reader[5].ToString());
                PICS = JsonConvert.DeserializeObject<List<string>>(reader[6].ToString());

                inPlacementScheme = JsonConvert.DeserializeObject<VectorPic>(reader[7].ToString());
                inBox = JsonConvert.DeserializeObject<VectorPic>(reader[8].ToString());
                inConnectionScheme = JsonConvert.DeserializeObject<VectorPic>(reader[9].ToString());
                inStructural = JsonConvert.DeserializeObject<VectorPic>(reader[10].ToString());
                bytedFile = JsonConvert.DeserializeObject<byte[]>(reader[11].ToString());
                isWire = Convert.ToBoolean(reader[12].ToString());

            }
            catch (Exception ex)
            {

            }
        }
        public Equipment(int id, string name, string description, string preview, Dictionary<string, string> properties, List<Compatibility> compatibilities, List<string> PICS, VectorPic inPlacementScheme, VectorPic inBox, VectorPic inConnectionScheme, VectorPic inStructural, byte[] bytedFile, bool isWire)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.preview = preview;

            this.properties = properties;
            this.compatibilities = compatibilities;
            this.PICS = PICS;

            this.inPlacementScheme = inPlacementScheme;
            this.inBox = inBox;
            this.inConnectionScheme = inConnectionScheme;
            this.inStructural = inStructural;
            this.bytedFile = bytedFile;
            this.isWire = isWire;

        }
        public Equipment() { }
        public class Compatibility
        {
            public InterfaceType interfaceType;
            public int count;
            public bool isMama;
        }
        public class VectorPic
        {
            public List<List<Point>> polyLines;
            //public List<Point> hatching;
            public List<circle> circles;
            //public List<arc> arcs;
            public VectorPic copy()
            {
                VectorPic cop = new VectorPic();
                cop.polyLines = new List<List<Point>>();
                foreach (var i in polyLines)
                {
                    cop.polyLines.Add(new List<Point>());
                    foreach(var j in i)
                    {
                        cop.polyLines[cop.polyLines.Count - 1].Add(j.copy());
                    }
                }
                //cop.hatching = new List<Point>();
                //foreach (var i in hatching) cop.hatching.Add(i.copy());

                    cop.circles = new List<circle>();
                foreach (var i in circles)
                    cop.circles.Add(i.copy());
                //cop.arcs = new List<arc>();
                //foreach (var i in arcs)
                //    cop.arcs.Add(i.copy());
                return cop;
            }
            public Point GetProp()
            {
                double LeftUpperX=0, LeftUpperY = 0, RightLowerX = 0, RightLowerY = 0;
                //if (arcs.Count != 0)
                //{
                //    LeftUpperX = arcs[0].UpperLeft.X; LeftUpperY = arcs[0].UpperLeft.Y;
                //    RightLowerX = arcs[0].UpperLeft.X; RightLowerY = arcs[0].UpperLeft.Y;
                //}
                if (polyLines.Count != 0)
                {
                    LeftUpperX = polyLines[0][0].X; LeftUpperY = polyLines[0][0].Y;
                    RightLowerX = polyLines[0][0].X; RightLowerY = polyLines[0][0].Y;
                }
                else if (circles.Count != 0)
                {
                    LeftUpperX = circles[0].center.X; LeftUpperY = circles[0].center.Y;
                    RightLowerX = circles[0].center.X; RightLowerY = circles[0].center.Y;
                }
                foreach (var i in polyLines)
                {
                    foreach (var j in i)
                    {
                        if (j.X < LeftUpperX)
                            LeftUpperX = j.X;
                        if (j.X > RightLowerX)
                            RightLowerX = j.X;

                        if (j.Y < LeftUpperY)
                            LeftUpperY = j.Y;
                        if (j.Y > RightLowerY)
                            RightLowerY = j.Y;
                    }
                }
                foreach (var i in circles)
                {
                    if (i.center.X - i.radius < LeftUpperX)
                        LeftUpperX = i.center.X - i.radius;
                    if (i.center.X + i.radius > RightLowerX)
                        RightLowerX = i.center.X + i.radius;

                    if (i.center.Y - i.radius < LeftUpperY)
                        LeftUpperY = i.center.Y - i.radius;
                    if (i.center.Y + i.radius > RightLowerY)
                        RightLowerY = i.center.Y + i.radius;
                }
                //foreach (var i in arcs)
                //{
                //    if (i.UpperLeft.X < LeftUpperX)
                //        LeftUpperX = i.UpperLeft.X;
                //    if (i.UpperLeft.X + i.radius * 2 > RightLowerX)
                //        RightLowerX = i.UpperLeft.X + i.radius * 2;

                //    if (i.UpperLeft.Y < LeftUpperY)
                //        LeftUpperY = i.UpperLeft.Y;
                //    if (i.UpperLeft.Y + i.radius * 2 > RightLowerY)
                //        RightLowerY = i.UpperLeft.Y + i.radius * 2;
                //}
                return new Point() { X = (float)RightLowerX - (float)LeftUpperX, Y = (float)RightLowerY - (float)LeftUpperY };
            }
            public void divide(float x, float y)
            {
                if (polyLines != null)
                    foreach (var i in polyLines)
                        foreach (var j in i)
                            j.divide(x,y);
                //if (hatching != null)
                //    foreach (var i in hatching)
                //        i.divide(x, y);
                if (circles != null)
                    foreach (var i in circles)
                        i.divide(x, y);
                //if (arcs != null)
                //    foreach (var i in arcs)
                //        i.divide(x, y);
            }
        }
        public class Point
        {
            public float X, Y;
            public void divide(float a)
            {
                X /= a;
                Y /= a;

            }
            public void divide(float a, float b)
            {
                X /= a;
                Y /= b;

            }
            public Point copy()
            {
                return new Point() {  X= X, Y=Y};
            }
        }
        public enum Stage { first, second, third }
//        public static VectorPic vectorization(Bitmap img)
//        {
//            //debugger bug = new debugger();
//            //bug.Show();
//            //bug.Location = new Point(0, 0);
//            VectorPic pic = new VectorPic();
//            //пятна
//            List<List<Point>> spots = new List<List<Point>>();
//            List<pixel> veryfied = new List<pixel>(), suspected = new List<pixel>();
//            for (int i = 1; i < img.Width - 1; i++)
//            {
//                for (int j = 1; j < img.Height - 1; j++)
//                {
//                    if (veryfied.Exists(x => x.a == i && x.b == j)) continue;
//                    bool good = true;
//                    for (int ii = i - 1; ii < i - 1 + 3; ii++)
//                    {
//                        for (int jj = j - 1; jj < j - 1 + 3; jj++)
//                        {
//                            if (colorEquals(img.GetPixel(ii, jj),Color.White))
//                            {
//                                good = false;
//                                ii = int.MaxValue;
//                                break;
//                            }
//                        }
//                        if (ii == int.MaxValue) break;
//                    }
//                    if (good) //найдено новое пятно
//                    {
//                        spots.Add(new List<Point>());
//                        veryfied.Add(new pixel() { a = i, b = j });
//                        /**/
//                        //bug.draw(i, j);
//                        /**/
//                        spots[spots.Count - 1].Add(new Point() { X = i, Y = j });
//                        for (int ii = i - 1; ii < i - 1 + 3; ii++)
//                        {
//                            for (int jj = j - 1; jj < j - 1 + 3; jj++)
//                            {
//                                if (veryfied.Exists(x => x.a == ii && x.b == jj)) continue;
//                                if (suspected.Exists(x => x.a == ii && x.b == jj)) continue;
//                                suspected.Add(new pixel() { a = ii, b = jj });
//                            }
//                        }

//                        while (suspected.Count != 0)
//                        {
//                            for (int t = 0; t < suspected.Count; t++)
//                            {
//                                {
//                                    good = true;
//                                    for (int ii = suspected[t].a - 1; ii < suspected[t].a - 1 + 3; ii++)
//                                    {
//                                        for (int jj = suspected[t].b - 1; jj < suspected[t].b - 1 + 3; jj++)
//                                        {
//                                            try
//                                            {
//                                                if (colorEquals(img.GetPixel(ii, jj), Color.White))
//                                                {
//                                                    good = false;
//                                                    ii = int.MaxValue;
//                                                    break;
//                                                }
//                                            }catch(Exception ex)
//                                            {
//                                                good = false;
//                                                ii = int.MaxValue;
//                                                break;
//                                            }
//                                        }
//                                        if (ii == int.MaxValue) break;
//                                    }
//                                    veryfied.Add(suspected[t]);
//                                    /**/
//                                    //bug.draw(suspected[t].a, suspected[t].b);
//                                    /**/
//                                    spots[spots.Count - 1].Add(new Point() { X = suspected[t].a, Y = suspected[t].b });
//                                    if (good)
//                                    {
//                                        for (int ii = suspected[t].a - 1; ii < suspected[t].a - 1 + 3; ii++)
//                                        {
//                                            for (int jj = suspected[t].b- 1; jj < suspected[t].b - 1 + 3; jj++)
//                                            {
//                                                if (veryfied.Exists(x => x.a == ii && x.b == jj)) continue;
//                                                if (suspected.Exists(x => x.a == ii && x.b == jj)) continue;
//                                                suspected.Add(new pixel() { a = ii, b = jj });
//                                            }
//                                        }
//                                    }
//                                    suspected.RemoveAt(t);
//                                    t--;
//                                }
//                            }
//                        }
//                    }
                  
//                }
//            }
//            //calculating border lines
//            pic.hatching = new List<List<List<Point>>>();
//            foreach (var i in spots)
//                i.RemoveAll(x => isBlacked(x, img));
//            ////////построение самых больших ниток, максимум 2 штуки
//            int started = 0;
//            for (int i = 0; i < spots.Count; i++)
//            {
//                pic.hatching.Add(new List<List<Point>>());
//                pic.hatching[pic.hatching.Count - 1].Add(new List<Point>());
//                int prev = 0, count = 0;
//                foreach (var j in spots[i])
//                {
                    
//                    if (count == 10) break;
//                    sameCount = 0;
//                    findLongestStripe(j.X, j.Y, ref pic, new Point[] { j }.ToList(), spots[i], 0, spots[i].Count);
//                    if (prev == pic.hatching[i][0].Count) count++;
//                    else
//                    {
//                        prev = pic.hatching[i][0].Count;
//                        count = 0;
//                    }
//                    if (pic.hatching[i][0].Count == spots[i].Count) break;
//                }
//                if (pic.hatching[i][0].Count < spots[i].Count) //найдено кольцо
//                {
//                    //поиск внутреннего/внешнего периметра
//                    pic.hatching[pic.hatching.Count - 1].Add(new List<Point>());
//                    prev = 0; count = 0;
//                    //new array with no visited
//                    List<Point> list = new List<Point>();
//                    foreach (var j in spots[i])
//                        if (!pic.hatching[i][0].Exists(x => x.X == j.X && x.Y == j.Y))
//                            list.Add(j);
//                    foreach (var j in list)
//                    {
//                        //if (pic.hatching[i][0].Exists(x => x.X == j.X && x.Y == j.Y))
//                        //    continue;
//                        if (count == 10) break;
//                        sameCount = 0;
//                        List<Point> newVisited = new List<Point>(pic.hatching[i][0]);
//                        newVisited.Add(new Point() { X=j.X, Y=j.Y });
//                        findLongestStripe(j.X, j.Y, ref pic, newVisited, list, 1, spots[i].Count, pic.hatching[i][0].Count);
//                        if (prev == pic.hatching[i][1].Count) count++;
//                        else
//                        {
//                            prev = pic.hatching[i][1].Count;
//                            count = 0;
//                        }
//                        if (pic.hatching[i][0].Count + pic.hatching[i][1].Count == spots[i].Count) break;
//                    }
//                }

//                started = 0;
//                //}
//                if(pic.hatching[pic.hatching.Count - 1].Count == 2)
//                {
//                    if(pic.hatching[pic.hatching.Count - 1][0].Count< pic.hatching[pic.hatching.Count - 1][1].Count)
//                    {
//                        List<Point> list = new List<Point>( pic.hatching[pic.hatching.Count - 1][0]);
//                        pic.hatching[pic.hatching.Count - 1][0] = new List<Point>(pic.hatching[pic.hatching.Count - 1][1]);
//                        pic.hatching[pic.hatching.Count - 1][1] = list;
//                    }
//                }
//            }
//            //**//
//            int ccount = 0;
//            for (int i = 0; i < pic.hatching.Count; i++)
//                for (int j = 0; j < pic.hatching[i].Count; j++)
//                    ccount += pic.hatching[i][j].Count;
//            /**/
//                        ////////////////////Сборка в линии
//            for (int i = 0; i < pic.hatching.Count; i++)
//            {
//                for(int j = 0; j < pic.hatching[i].Count; j++)
//                {
//                    for (int k = 0; k < pic.hatching[i][j].Count-1; k++)
//                    {
//                        //поимка линии
//                        float X = pic.hatching[i][j][k].X, Y = pic.hatching[i][j][k].Y;
//                        bool isEqualByX = false;
//                        float newX = pic.hatching[i][j][k+1].X, newY = pic.hatching[i][j][k+1].Y;
//                        float propX = newX - (X * 1.0f), propY = newY - (Y * 1.0f);
//                        //if (X == newX) isEqualByX = true;
//                        //else if (Y == newY) isEqualByX = false;
//                        //else continue;

//                        int lastIndex = k + 1;
//                        for (int kk = k + 2; kk < pic.hatching[i][j].Count; kk++)
//                        {
//                            X = newX; Y = newY;
//                            newX = pic.hatching[i][j][kk].X;
//                            newY = pic.hatching[i][j][kk].Y;

//                            //if (isEqualByX)
//                            //{
//                            if (Math.Round(newX - (X * 1.0f), 2).ToString() == Math.Round(propX, 2).ToString() && Math.Round(newY - (Y * 1.0f), 2).ToString() == Math.Round(propY, 2).ToString())
//                                lastIndex++;
//                            else if (kk == pic.hatching[i][j].Count - 1)
//                                lastIndex++;
//                            else
//                            {
//                                newX = pic.hatching[i][j][kk + 1].X;
//                                newY = pic.hatching[i][j][kk + 1].Y;
//                                if (Math.Round(newX - (X * 1.0f), 2).ToString() == Math.Round(propX, 2).ToString() && Math.Round(newY - (Y * 1.0f), 2).ToString() == Math.Round(propY, 2).ToString())
//                                    lastIndex++;
//                                else
//                                    break;
//                            }
//                            //}
//                            //else
//                            //{
//                            //if (Y == newY) lastIndex++;
//                            //else break;
//                            //}
//                        }
//                        //удаление всего что между k и lastIndex
//                        for (int kk=k+1;kk< lastIndex; kk++)
//                            pic.hatching[i][j].RemoveAt(k + 1);
//                    }
//                }
//            }
//            ccount = 0;
//            for (int i = 0; i < pic.hatching.Count; i++)
//                for (int j = 0; j < pic.hatching[i].Count; j++)
//                    ccount += pic.hatching[i][j].Count;
//            /**/
//            ////////////////////////
//            //calculating polylines
//            pic.polyLines = new List<List<Point>>();
//            for (int i = 0; i < img.Width; i++)
//            {
//                for (int j = 0; j < img.Height; j++)
//                {
//                    if (!colorEquals(img.GetPixel(i, j),Color.White) && !veryfied.Exists(x => x.a == i && x.b == j))
//                    {
//                        //founded
//                        //buliding polyline
//                        pic.polyLines.Add(new List<Point>());
//                        pic.polyLines[pic.polyLines.Count - 1].Add(new Point() { X = i, Y = j });
//                        veryfied.Add(new pixel() { a = i, b = j });
//                        /**/
//                        //bug.drawBlue(i, j);
//                        /**/
//                        pixel curr = new pixel() { a = i, b = j };
//                        while (hasBlackAround(curr, ref veryfied, img))
//                        {
//                            //find nearest black pixel
//                            float mindist = float.MaxValue;
//                            float num_a=0, num_b=0;
//                            for (float ii = curr.a - 1; ii < curr.a - 1 + 3; ii++)
//                                for (float jj = curr.b - 1; jj < curr.b - 1 + 3; jj++)
//                                {
//                                    try
//                                    {
//                                        if (!colorEquals(img.GetPixel((int)ii, (int)jj), Color.White) && !veryfied.Exists(x => x.a == ii && x.b == jj))
//                                        {
//                                            float dist = (float)Math.Sqrt(Math.Pow(curr.a - ii, 2) + Math.Pow(curr.b - jj, 2));
//                                            if (dist < mindist)
//                                            {
//                                                mindist = dist;
//                                                num_a = ii; num_b = jj;
//                                            }
//                                        }
//                                    }
//                                    catch (Exception ex) { }
//                                }
//                            pic.polyLines[pic.polyLines.Count - 1].Add(new Point() { X = num_a, Y = num_b });
//                            curr = new pixel() { a=(int)num_a, b=(int)num_b};
//                            veryfied.Add(curr);
//                            /**/
//                            //bug.drawBlue(num_a, num_b);
//                            /**/
//                        }
//                    }
//                }
//            }
//            //// сборка в линии
//            ////DELETING DOTS
//            pic.polyLines.RemoveAll(x => x.Count == 1);
//            /////

//            ccount = 0;
//            for (int i = 0; i < pic.polyLines.Count; i++)
//                ccount += pic.polyLines[i].Count;

//            foreach (var i in pic.polyLines)
//            {
//                for(int j = 0; j < i.Count-1; j++)
//                {
//                    float X = i[j].X, Y = i[j].Y;
//                    //bool isEqualByX = false;
//                    float newX = i[j+1].X, newY = i[j+1].Y;

//                    float propX = newX - X, propY = newY - Y * 1.0f;
//                    //if (X == newX) isEqualByX = true;
//                    //else if (Y == newY) isEqualByX = false;
//                    //else continue;

//                    int lastIndex = j + 1;
//                    for (int kk = j + 2; kk < i.Count; kk++)
//                    {
//                        X = newX; Y = newY;
//                        newX = i[kk].X;
//                        newY = i[kk].Y;

//                        //if (isEqualByX)
//                        //{
//                        if (Math.Round(newX-(X*1.0f),2).ToString() == Math.Round(propX,2).ToString() && Math.Round(newY-(Y*1.0f),2).ToString() == Math.Round(propY,2).ToString())
//                            lastIndex++;
//                        else if (kk == i.Count - 1)
//                            lastIndex++;
//                        else
//                        {
//                            newX = i[kk + 1].X;
//                            newY = i[kk + 1].Y;
//                            if (Math.Round(newX - (X * 1.0f), 2).ToString() == Math.Round(propX, 2).ToString() && Math.Round(newY - (Y * 1.0f), 2).ToString() == Math.Round(propY, 2).ToString())
//                                lastIndex++;
//                            else
//                                break;
//                        }
//                        //}
//                        //else
//                        //{
//                        //if (Y == newY) lastIndex++;
//                        //else break;
//                        //}
//                    }
//                    //удаление всего что между k и lastIndex
//                    for (int kk = j + 1; kk < lastIndex; kk++)
//                        i.RemoveAt(j + 1);
//                }
//            }
//            ccount = 0;
//            for (int i = 0; i < pic.polyLines.Count; i++)
//                ccount += pic.polyLines[i].Count;
//            //  foreach (var i in veryfied) pic.hatching[0].Add(new Point() { X = i.a, Y = i.b });
//            return pic;
//        }
        static int sameCount = 0;
        //static void findLongestStripe(float X, float Y, ref VectorPic result, List<Point> visited, List<Point> borders, int num, int bordersCount, int prevVisited = 0)
        //{
        //    if (sameCount == 10) return;
        //    int sum = 0;
        //    foreach (var i in result.hatching[result.hatching.Count - 1])
        //        sum += i.Count;
        //    if (sum == bordersCount)
        //    {
        //        if (result.hatching[result.hatching.Count - 1][num].Count < visited.Count-prevVisited)
        //        {
        //            result.hatching[result.hatching.Count - 1][num] = new List<Point>();
        //            for (int i = prevVisited; i < visited.Count; i++)
        //                result.hatching[result.hatching.Count - 1][num].Add(visited[i]);
        //        }
        //        return;
        //    }
        //        while (true)
        //    {
        //        List<Point> nextLocations = new List<Point>();
        //        for (float i = X - 1; i < X - 1 + 3; i++)
        //            for (float j = Y - 1; j < Y - 1 + 3; j++)
        //                if (borders.Exists(x => x.X == i && x.Y == j) && !visited.Exists(x => x.X == i && x.Y == j))
        //                    nextLocations.Add(new Point() { X = i, Y = j });
        //        if (nextLocations.Count == 0)
        //        {
        //            sameCount++;
        //            if (result.hatching[result.hatching.Count - 1][num].Count < visited.Count-prevVisited)
        //            {
        //                result.hatching[result.hatching.Count - 1][num] = new List<Point>();
        //                for (int i = prevVisited; i < visited.Count; i++)
        //                    result.hatching[result.hatching.Count - 1][num].Add(visited[i]);
        //                sameCount = 0;
        //            }
        //            return;
        //        }
        //        if (nextLocations.Count == 1)
        //        {
        //            X = nextLocations[0].X;
        //            Y = nextLocations[0].Y;
        //            visited.Add(new Point() { X = X, Y = Y });
        //        }
        //      else
        //        {
        //          //  visited_copy.AddRange(visited);

        //            foreach (var i in nextLocations)
        //            {
        //                List<Point> visited_copy = new List<Point>();
        //                foreach (var j in visited) visited_copy.Add(j);
        //                visited_copy.Add(i);
        //                findLongestStripe(X, Y, ref result, new List<Point>(visited_copy), borders, num, prevVisited);
        //            }
        //            return;
        //        }
        //    }
        //}
        static bool equals(Point a, Point b)
        {
            //Point newb = new Point() { X = b.a, Y = b.b };
            return a.X == b.X && a.Y == b.Y;
        }
        static bool hasBlackAround(pixel curr, ref List<pixel> veryfied, Bitmap img)
        {
            for (float i = curr.a - 1; i < curr.a - 1 + 3; i++)
                for (float j = curr.b - 1; j < curr.b - 1 + 3; j++)
                {
                    try
                    {
                        if (!colorEquals(img.GetPixel((int)i, (int)j), Color.White) && !veryfied.Exists(x => x.a == i && x.b == j))
                            return true;
                    }
                    catch (Exception ex) { }
                }
            return false;
        }
        static bool isBlacked(Point a, Bitmap img)
        {
            for (float i = a.X - 1; i < a.X - 1 + 3; i++)
                for (float j = a.Y - 1; j < a.Y - 1 + 3; j++)
                {
                    try
                    {
                        if (colorEquals(img.GetPixel((int)i, (int)j), Color.White))
                            return false;
                    }catch (Exception ex)
                    {
                        return false;
                    }
                }
            return true;
        }
        public static bool colorEquals(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }
        struct pixel { public int a, b;
            public override bool Equals(object obj)
            {
                pixel other = (pixel)obj;
                return other.a == a && other.b == b;
            }
        }
    }



    public class dot
    {
        public double X, Y;
        public dot copy()
        {
            return new dot() { X = X, Y = Y };
        }
    }
    public class pline
    {
        public List<dot> list;
    }
    //public class arc
    //{
    //    public dot UpperLeft;
    //    public double radius;
    //    public double radiusX, radiusY;
    //    public double startAngle, EndAngle;
    //    public void divide(float a)
    //    {
    //        UpperLeft.X /= a;
    //        UpperLeft.Y /= a;
    //        radiusX /= a;
    //        radiusY /= a;
    //        radius /= a;
    //    }
    //    public void divide(float a, float b)
    //    {
    //        UpperLeft.X /= a;
    //        UpperLeft.Y /= b;
    //        radiusX /= a;
    //        radiusY /= b;
    //        if (radiusX > radiusY) radius = radiusX;
    //        else radius = radiusY;
    //    }
    //    public arc copy()
    //    {
    //        arc newa = new arc() { EndAngle = EndAngle, radiusX = radiusX, radiusY = radiusY, radius = radius, startAngle = startAngle, UpperLeft = UpperLeft.copy() };
    //        return newa;
    //    }
    //}
    public class circle
    {
        public dot center;
        public double radiusX, radiusY;
        public double radius;
        public void setR(double r)
        {
            radiusX = r;
            radiusY = r;
            radius = r;
        }
        public double getR()
        {
            return radius;
        }
        public void divide(float a)
        {
            center.X /= a;
            center.Y /= a;
            radiusX /= a;
            radiusY /= a;
            radius /= a;
        }
        public void divide(float a, float b)
        {
            center.X /= a;
            center.Y /= b;
            radiusX /= a;
            radiusY /= b;
            if (radiusX > radiusY) radius = radiusX;
            else radius = radiusY;
        }
        public circle copy()
        {
            circle nec = new circle() { center = center.copy(), radiusX = radiusX, radiusY = radiusY, radius = radius };
            return nec;
        }
    }
    public class image
    {
        public List<circle> circles = new List<circle>();
        //public List<arc> arcs = new List<arc>();
        public List<pline> plines = new List<pline>();
    }
}
