using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VotacionesWebSite.Models;

namespace VotacionesWebSite.Controllers
{
    [Authorize]
    public class VotingsController : Controller
    {
        private VotacionesContext db = new VotacionesContext();

        public ActionResult AddGroup(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.GroupId = new SelectList(db.Groups.OrderBy(p => p.Description), "GroupId", "Description");

            var votingGroup = new VotingGroup
            {
                VotingId = id
            };

            return View(votingGroup);
        }

        [HttpPost]
        public ActionResult AddGroup(VotingGroup view)
        {
            if (ModelState.IsValid)
            {
                var votingGroup = db.VotingGroups.Where(p => p.VotingId == view.VotingId && p.GroupId == view.GroupId).FirstOrDefault();

                if (votingGroup != null)
                {
                    ViewBag.Error = "The group already belongs to voting";
                    ViewBag.GroupId = new SelectList(db.Groups.OrderBy(p => p.Description), "GroupId", "Description", view.GroupId);
                    return View(view);
                }

                db.VotingGroups.Add(view);
                db.SaveChanges();
                return RedirectToAction(string.Format("Details/{0}", view.VotingId));
            }

            ViewBag.GroupId = new SelectList(db.Groups.OrderBy(p => p.Description), "GroupId", "Description", view.GroupId);

            return View(view);
        }


        // GET: Votings
        public ActionResult Index()
        {
            var votings = db.Votings.Include(v => v.State);
            return View(votings.ToList());
        }

        // GET: Votings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }
            return View(voting);
        }

        // GET: Votings/Create
        public ActionResult Create()
        {
            ViewBag.StateId = new SelectList(db.States.OrderBy(p => p.Description), "StateId", "Description");
            return View();
        }

        // POST: Votings/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "votingId,Description,StateId,Remarks,DateTimeStart,DateTimeEnd,IsForAllUsers,IsEnableBlankVote,QuantityVotes,QuantityBlankVotes,CandidateWinId")] Voting voting)
        {
            if (ModelState.IsValid)
            {
                db.Votings.Add(voting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", voting.StateId);
            return View(voting);
        }

        // GET: Votings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", voting.StateId);
            return View(voting);
        }

        // POST: Votings/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "votingId,Description,StateId,Remarks,DateTimeStart,DateTimeEnd,IsForAllUsers,IsEnableBlankVote,QuantityVotes,QuantityBlankVotes,CandidateWinId")] Voting voting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", voting.StateId);
            return View(voting);
        }

        // GET: Votings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }
            return View(voting);
        }

        // POST: Votings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voting voting = db.Votings.Find(id);
            db.Votings.Remove(voting);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
