using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using kagakugijutu.Models;

namespace kagakugijutu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _filePath = "App_Data/posts.txt";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // 投稿保存用フォルダがなければ作成
            if (!System.IO.Directory.Exists("App_Data"))
            {
                System.IO.Directory.CreateDirectory("App_Data");
            }
        }

        // 投稿一覧表示（一般）
        public IActionResult Index()
        {
            List<string> posts = new();

            if (System.IO.File.Exists(_filePath))
            {
                posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();
            }

            return View(posts);
        }

        // 投稿フォーム表示
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 投稿保存処理
        [HttpPost]
        public IActionResult Create(string userName, string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string namePart = string.IsNullOrWhiteSpace(userName) ? "匿名" : userName;
                string entry = $"{timestamp} | {namePart} | {comment}";
                System.IO.File.AppendAllText(_filePath, entry + Environment.NewLine, Encoding.UTF8);
            }

            return RedirectToAction("Index");
        }

        // 教員専用ログイン画面
        [HttpGet]
        public IActionResult Teacher()
        {
            return View();
        }

        // 教員専用パスワード処理
        [HttpPost]
        public IActionResult Teacher(string password)
        {
            if (password == "123") // ← 任意のパスワードに変更可能
            {
                return RedirectToAction("TeacherDashboard");
            }

            ViewBag.Error = "パスワードが正しくありません。";
            return View();
        }

        // 教員専用ページ表示
        public IActionResult TeacherDashboard()
        {
            List<string> posts = new();

            if (System.IO.File.Exists(_filePath))
            {
                posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();
            }

            return View(posts);
        }

        // 投稿を個別削除
        [HttpPost]
        // 新：複数選択された投稿を削除
        [HttpPost]
        public IActionResult DeleteSelected(List<int> deleteIndexes)
        {
            if (deleteIndexes != null && deleteIndexes.Any())
            {
                List<string> posts = new();

                if (System.IO.File.Exists(_filePath))
                {
                    posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();

                    // インデックスを降順にしてから削除（位置ずれ防止）
                    foreach (var index in deleteIndexes.OrderByDescending(i => i))
                    {
                        if (index >= 0 && index < posts.Count)
                        {
                            posts.RemoveAt(index);
                        }
                    }

                    System.IO.File.WriteAllLines(_filePath, posts, Encoding.UTF8);
                }
            }

            return RedirectToAction("TeacherDashboard");
        }


        // 指定日以前の投稿を一括削除
        [HttpPost]
        public IActionResult DeleteBefore(string cutoffDate)
        {
            if (DateTime.TryParse(cutoffDate, out var cutoff))
            {
                List<string> posts = new();

                if (System.IO.File.Exists(_filePath))
                {
                    posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8)
                        .Where(line =>
                        {
                            var parts = line.Split(" - ");
                            return parts.Length < 2 || !DateTime.TryParse(parts[0], out var postDate) || postDate >= cutoff;
                        })
                        .ToList();

                    System.IO.File.WriteAllLines(_filePath, posts, Encoding.UTF8);
                }
            }

            return RedirectToAction("TeacherDashboard");
        }

        // その他テンプレート保持（任意）
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
