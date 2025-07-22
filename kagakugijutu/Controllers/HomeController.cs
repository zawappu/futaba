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

            // ���e�ۑ��p�t�H���_���Ȃ���΍쐬
            if (!System.IO.Directory.Exists("App_Data"))
            {
                System.IO.Directory.CreateDirectory("App_Data");
            }
        }

        // ���e�ꗗ�\���i��ʁj
        public IActionResult Index()
        {
            List<string> posts = new();

            if (System.IO.File.Exists(_filePath))
            {
                posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();
            }

            return View(posts);
        }

        // ���e�t�H�[���\��
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ���e�ۑ�����
        [HttpPost]
        public IActionResult Create(string userName, string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string namePart = string.IsNullOrWhiteSpace(userName) ? "����" : userName;
                string entry = $"{timestamp} | {namePart} | {comment}";
                System.IO.File.AppendAllText(_filePath, entry + Environment.NewLine, Encoding.UTF8);
            }

            return RedirectToAction("Index");
        }

        // ������p���O�C�����
        [HttpGet]
        public IActionResult Teacher()
        {
            return View();
        }

        // ������p�p�X���[�h����
        [HttpPost]
        public IActionResult Teacher(string password)
        {
            if (password == "123") // �� �C�ӂ̃p�X���[�h�ɕύX�\
            {
                return RedirectToAction("TeacherDashboard");
            }

            ViewBag.Error = "�p�X���[�h������������܂���B";
            return View();
        }

        // ������p�y�[�W�\��
        public IActionResult TeacherDashboard()
        {
            List<string> posts = new();

            if (System.IO.File.Exists(_filePath))
            {
                posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();
            }

            return View(posts);
        }

        // ���e���ʍ폜
        [HttpPost]
        // �V�F�����I�����ꂽ���e���폜
        [HttpPost]
        public IActionResult DeleteSelected(List<int> deleteIndexes)
        {
            if (deleteIndexes != null && deleteIndexes.Any())
            {
                List<string> posts = new();

                if (System.IO.File.Exists(_filePath))
                {
                    posts = System.IO.File.ReadAllLines(_filePath, Encoding.UTF8).ToList();

                    // �C���f�b�N�X���~���ɂ��Ă���폜�i�ʒu����h�~�j
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


        // �w����ȑO�̓��e���ꊇ�폜
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

        // ���̑��e���v���[�g�ێ��i�C�Ӂj
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
