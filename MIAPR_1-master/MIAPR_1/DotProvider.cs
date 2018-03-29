using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MIAPR_1
{
    internal delegate void ShowDots(Dictionary<Point, List<Point>> classes);

    internal class DotProvider
    {
        private static List<Point> GenerateDots(int dotCount, int maxWidth, int maxHeight)
        {
            List<Point> pointList = new List<Point>();
            var random = new Random();
            for (int i = 0; i<dotCount; i++)
            {
                var point = new Point();
                point.X = random.Next() % maxWidth;
                point.Y = random.Next() % maxHeight;
                pointList.Add(point);
            }
            return pointList;
        }

        private static List<Point> GenerateClassCenters(int classCount, List<Point> pointList)
        {
            List<Point> classCenters = new List<Point>();
            var random = new Random();
            var set = new HashSet<int>();
            int i = 0;
            while (i < classCount)
            {
                int centerNum = random.Next() % classCount;
                if (!set.Contains(centerNum))
                {
                    set.Add(centerNum);
                    Point centerPoint = pointList[centerNum];
                    centerPoint.classNum = i;
                    classCenters.Add(centerPoint);
                    i++;
                }
            }
            return classCenters;
        }

        private static double GetDistance(Point point1, Point point2)
        {
            int x = point1.X - point2.X;
            int y = point1.Y - point2.Y;
            return Math.Sqrt(x * x + y * y); 
        }

        private static double GetDeltaSqrt(Point center, List<Point> points)
        {
            double sum = 0;
            foreach (Point point in points)
            {
                if (point != center)
                {
                    double distance = GetDistance(center, point);
                    sum += distance * distance;
                }
            }
            return sum;
        }

        private static Dictionary<Point, List<Point>> CreateClasses(List<Point> points, List<Point> centers)
        {
            Dictionary<Point, List<Point>> classes = new Dictionary<Point, List<Point>>();
            foreach (Point center in centers)
            {
                classes.Add(center, new List<Point> { center });
            }
            foreach (Point point in points)
            {
                if (!centers.Contains(point))
                {
                    Point center = centers[0];
                    double minDistance = GetDistance(point, center);
                    for (int i = 1; i < centers.Count; i++)
                    {
                        double distance = GetDistance(point, centers[i]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            center = centers[i];
                        }
                    }
                    classes[center].Add(point);
                    point.classNum = center.classNum;
                }
            }
            return classes;
        }

        private static void QualifyDots(Dictionary<Point, List<Point>> classes)
        {
            var newCenters = new List<Point>();
            var addedDots = new List<Point>();
            var oldCenters = new List<Point>();
            Object syncObject = new object();
            List<Point> centers = classes.Keys.ToList();
            foreach (Point center in classes.Keys)
            {
                var points = classes[center];
                var tasks = new List<Task>();
                foreach (Point point in points)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        Point newCenter = center;
                        double minDistance = GetDistance(center, point);
                        foreach (Point diffCenter in centers)
                        {
                            if (diffCenter != center)
                            {
                                double distance = GetDistance(point, diffCenter);
                                if (distance < minDistance)
                                {
                                    newCenter = diffCenter;
                                    minDistance = distance;
                                }
                            }
                        }
                        if (newCenter != center)
                        {
                            lock (syncObject)
                            {
                                newCenters.Add(newCenter);
                                oldCenters.Add(center);
                                addedDots.Add(point);
                            }
                        }
                    }));
                }
                Task.WaitAll(tasks.ToArray());
            }
            for (int i = 0; i < newCenters.Count; i++)
            {
                classes[oldCenters[i]].Remove(addedDots[i]);
                classes[newCenters[i]].Add(addedDots[i]);
                addedDots[i].classNum = newCenters[i].classNum;
            }
        }

        private static bool RefreshCenters(Dictionary<Point, List<Point>> classes)
        {
            bool result = false;
            List<Point> oldCenters = new List<Point>();
            List<Point> newCenters = new List<Point>();
            object syncObject = new object();
            var tasks = new List<Task>();
            foreach (var pair in classes)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    List<Point> points = pair.Value;
                    Point oldCenter = pair.Key;
                    Point center = oldCenter;
                    double minSqrtDelta = GetDeltaSqrt(center, points);
                    foreach (Point point in points)
                    {
                        if (point != oldCenter)
                        {
                            double sqrtDelta = GetDeltaSqrt(point, points);
                            if (sqrtDelta < minSqrtDelta)
                            {
                                center = point;
                                minSqrtDelta = sqrtDelta;
                            }
                        }
                    }
                    if (center != oldCenter)
                    {
                        lock (syncObject)
                        {
                            oldCenters.Add(oldCenter);
                            newCenters.Add(center);
                            if (!result)
                            {
                                result = true;
                            }
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            for (int i = 0; i < oldCenters.Count; i++)
            {
                classes.Add(newCenters[i], classes[oldCenters[i]]);
                classes.Remove(oldCenters[i]);
            }
            return result;
        }

        public static void GetClasses(int dotCount, int classCount, int maxWidth, int maxHeight, ShowDots showDots)
        {
            List<Point> pointList = GenerateDots(dotCount, maxWidth, maxHeight);
            List<Point> classCenters = GenerateClassCenters(classCount, pointList);
            Dictionary<Point, List<Point>> classes = CreateClasses(pointList, classCenters);

            while (RefreshCenters(classes)) 
            {
                QualifyDots(classes);
                showDots(classes);
            }
        }
    }
}
