using BibliotecaApp.Data;
using BibliotecaApp.Extensions;
using BibliotecaApp.Models;
using BibliotecaApp.Validations;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Controllers.Web
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UsuariosController : Controller
    {
        private readonly BibliotecaAppContext _context;

        public UsuariosController(BibliotecaAppContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToArrayAsync());
            //return _context.Usuario != null ?
            //            View(await _context.Usuario.ToListAsync()) :
            //            Problem("Entity set 'BibliotecaAppContext.Usuario'  is null.");
        }


        // GET: Usuarios
        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            return _context.Usuario != null ?
                        View(await _context.Usuario.ToListAsync()) :
                        Problem("Entity set 'BibliotecaAppContext.Usuario'  is null.");
        }

        // GET: Usuarios/Details/5
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Email,Senha")] Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);

            ModelState.AddModelErrorIfNotEmpty("Nome", usuario.Nome.ValidarNome());
            ModelState.AddModelErrorIfNotEmpty("Email", usuario.Email.ValidarEmail());
            ModelState.AddModelErrorIfNotEmpty("Senha", usuario.Senha.ValidarSenha());
            if (EmailExists(usuario.Email))
            {
                ModelState.AddModelErrorIfNotEmpty("Email", "E-mail já existe no banco de dados");
            }

            if (ModelState.IsValid)
            {
                usuario.Id = Guid.NewGuid();
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [HttpGet("Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // PUT: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome,Email,Senha")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(usuario);

            ModelState.AddModelErrorIfNotEmpty("Nome", usuario.Nome.ValidarNome());
            ModelState.AddModelErrorIfNotEmpty("Email", usuario.Email.ValidarEmail());
            ModelState.AddModelErrorIfNotEmpty("Senha", usuario.Senha.ValidarSenha());
            if (EmailExists(usuario.Email))
            {
                ModelState.AddModelErrorIfNotEmpty("Email", "E-mail já existe no banco de dados");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Usuarios");
            }

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [HttpGet("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // DELETE: Usuarios/Delete/5
        [HttpPost("Delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Usuario == null)
            {
                return Problem("Entity set 'BibliotecaAppContext.Usuario'  is null.");
            }
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Usuarios");
            //return Json(new { success = true, redirectToUrl = Url.Action("Index", "Usuarios") });
        }

        private bool UsuarioExists(Guid id)
        {
            return (_context.Usuario?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool EmailExists(string email)
        {
            return (_context.Usuario?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}

