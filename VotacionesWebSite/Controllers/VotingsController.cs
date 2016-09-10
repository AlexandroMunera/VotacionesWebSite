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
    public class VotingsController : Controller
    {
        private VotacionesContext db = new VotacionesContext();

        [Authorize(Roles = "Admin")]
        public ActionResult CloseVoting(int id)
        {
            var voting = db.Votings.Find(id);

            if (voting != null)
            {
                var candidate = db.Candidates.Where(p => p.VotingId == id)
                    .OrderByDescending(p => p.QuantityVotes).FirstOrDefault();

                if (candidate != null)
                {
                    var state = GetState("Closed");
                    voting.StateId = state.StateId;
                    voting.CandidateWinId = candidate.UserId;
                    db.Entry(voting).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Results()
        {
            var state = this.GetState("closed");
            var votings = db.Votings.Where( p => p.StateId == state.StateId).Include(v => v.State);
            var views = new List<VotingIndexView>();
            var db2 = new VotacionesContext();
            foreach (var voting in votings)
            {
                User user = null;

                if (voting.CandidateWinId != 0)
                {
                    user = db2.Users.Find(voting.CandidateWinId);
                }

                views.Add(new VotingIndexView
                {
                    CandidateWinId = voting.CandidateWinId,
                    DateTimeEnd = voting.DateTimeEnd,
                    DateTimeStart = voting.DateTimeStart,
                    Description = voting.Description,
                    IsEnableBlankVote = voting.IsEnableBlankVote,
                    IsForAllUsers = voting.IsForAllUsers,
                    QuantityBlankVotes = voting.QuantityBlankVotes,
                    QuantityVotes = voting.QuantityVotes,
                    Remarks = voting.Remarks,
                    StateId = voting.StateId,
                    State = voting.State,
                    VotingId = voting.VotingId,
                    Winner = user,

                });
            }
            return View(views);
        }

        [Authorize(Roles = "User")]
        public ActionResult VoteForCandidate(int candidateId,int votingId)
        {
            var user = db.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                //Falta poner mensajes y devolver a myVottings
                return RedirectToAction("MyVotings");
            }

            var candidate = db.Candidates.Find(candidateId);
            if (candidate == null)
            {
                //Falta poner mensajes y devolver a myVottings
                return RedirectToAction("MyVotings");
            }

            var voting = db.Votings.Find(votingId);
            if (voting == null)
            {
                //Falta poner mensajes y devolver a myVottings
                return RedirectToAction("MyVotings");
            }

            if (VoteCandidate(user, candidate, voting))
            {
                return RedirectToAction("MyVotings");
            }


            return RedirectToAction("Index","Home");
        }

        private bool VoteCandidate(User user, Candidate candidate, Voting voting)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var votingDetail = new VotingDetail {
                    CandidateId = candidate.CandidateId,
                    DateTime = DateTime.Now,
                    UserId = user.UserId,
                    VotingId = voting.VotingId
                };

                db.VotingDetails.Add(votingDetail);

                candidate.QuantityVotes++;
                db.Entry(candidate).State = EntityState.Modified;

                voting.QuantityVotes++;
                db.Entry(voting).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }

                
            }
            return false;
        }

        [Authorize(Roles = "User")]
        public ActionResult Vote(int votingId)
        {
            var voting = db.Votings.Find(votingId);

            voting.MyCandidates = voting.Candidates.ToList();

            return View(voting);
        }

        [Authorize(Roles = "User")]
        public ActionResult MyVotings()
        {
            var user = db.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "There an error with the current user, call the support");
                return View();
            }

            // Get event votings for the current time
            var state = this.GetState("Open");


            var votings = db.Votings.Where(p => p.StateId == state.StateId &&
              p.DateTimeStart <= DateTime.Now && p.DateTimeEnd >= DateTime.Now)
              .Include(p => p.Candidates).Include(p => p.VotingGroups).Include(p => p.State).ToList();

            //Discard events in the wich the user already vote
            foreach (var voting in votings.ToList())
            {

                var votingDetail = db.VotingDetails.Where(p => p.VotingId == voting.VotingId && p.UserId == user.UserId).FirstOrDefault();

                if (votingDetail != null)
                {
                    votings.Remove(voting);
                }
            }

            ////Discard events by groups in wich the user are not include

            foreach (var voting in votings.ToList())
            {
                if (!voting.IsForAllUsers)
                {

                    bool userBelongsToGroup = false;

                    foreach (var votingGroup in voting.VotingGroups)
                    {
                        var userGroup = votingGroup.Group.GroupMembers.Where(p => p.UserId == user.UserId).FirstOrDefault();

                        if (userGroup != null)
                        {
                            userBelongsToGroup = true;
                            break;
                        }
                    }

                    if (!userBelongsToGroup)
                    {
                        votings.Remove(voting);
                    }

                }
            }

            return View(votings);
        }

        private State GetState(string stateName)
        {
            var state = db.States.Where(p => p.Description.Equals(stateName)).FirstOrDefault();

            if (state == null)
            {
                state = new State
                {
                    Description = stateName
                };

                db.States.Add(state);
                db.SaveChanges();

            }


            return state;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddCandidate(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var addCandidate = new AddCandidateView
            {
                VotingId = id
            };

            ViewBag.UserId = new SelectList(db.Users.OrderBy(p => p.FirstName).ThenBy(p => p.LastName), "UserId", "FullName");

            return View(addCandidate);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCandidate(AddCandidateView view)
        {
            if (ModelState.IsValid)
            {
                var candidate = db.Candidates.Where(p => p.VotingId == view.VotingId && p.UserId == view.UserId).FirstOrDefault();

                if (candidate != null)
                {
                    ModelState.AddModelError(string.Empty, "The candidate already belongs to voting.");
                    ViewBag.UserId = new SelectList(db.Users.OrderBy(p => p.FirstName).ThenBy(p => p.LastName), "UserId", "FullName");
                    return View(view);
                }

                candidate = new Candidate
                {
                    UserId = view.UserId,
                    VotingId = view.VotingId
                };

                db.Candidates.Add(candidate);
                db.SaveChanges();
                return RedirectToAction(string.Format("Details/{0}", view.VotingId));
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(p => p.FirstName).ThenBy(p => p.LastName), "UserId", "FullName");

            return View(view);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteGroup(int id)
        {
            var votingGroup = db.VotingGroups.Find(id);

            if (votingGroup != null)
            {
                db.VotingGroups.Remove(votingGroup);
                db.SaveChanges();
            }

            return RedirectToAction(string.Format("Details/{0}", votingGroup.VotingId));
        }


        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult AddGroup(VotingGroup view)
        {
            if (ModelState.IsValid)
            {
                var votingGroup = db.VotingGroups.Where(p => p.VotingId == view.VotingId && p.GroupId == view.GroupId).FirstOrDefault();

                if (votingGroup != null)
                {
                    ModelState.AddModelError(string.Empty, "The group already belongs to voting.");
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

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCandidate(int id)
        {
            var candidate = db.Candidates.Find(id);

            if (candidate != null)
            {
                db.Candidates.Remove(candidate);
                db.SaveChanges();
            }

            return RedirectToAction(string.Format("Details/{0}", candidate.VotingId));
        }

        // GET: Votings
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var votings = db.Votings.Include(v => v.State);
            var views = new List<VotingIndexView>();
            var db2 = new VotacionesContext();
            foreach (var voting in votings)
            {
                User user = null;

                if (voting.CandidateWinId != 0)
                {
                    user = db2.Users.Find(voting.CandidateWinId);
                }

                views.Add(new VotingIndexView
                {
                    CandidateWinId = voting.CandidateWinId,
                    DateTimeEnd = voting.DateTimeEnd,
                    DateTimeStart = voting.DateTimeStart,
                    Description = voting.Description,
                    IsEnableBlankVote = voting.IsEnableBlankVote,
                    IsForAllUsers = voting.IsForAllUsers,
                    QuantityBlankVotes = voting.QuantityBlankVotes,
                    QuantityVotes = voting.QuantityVotes,
                    Remarks = voting.Remarks,
                    StateId = voting.StateId,
                    State = voting.State,
                    VotingId = voting.VotingId,
                    Winner = user,

                });
            }

            return View(views);
        }

        // GET: Votings/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }

            var detailsVotingView = new DetailsVotingView
            {
                Candidates = voting.Candidates.ToList(),
                CandidateWinId = voting.CandidateWinId,
                DateTimeStart = voting.DateTimeStart,
                DateTimeEnd = voting.DateTimeEnd,
                Description = voting.Description,
                IsEnableBlankVote = voting.IsEnableBlankVote,
                IsForAllUsers = voting.IsForAllUsers,
                QuantityBlankVotes = voting.QuantityBlankVotes,
                QuantityVotes = voting.QuantityVotes,
                Remarks = voting.Remarks,
                State = voting.State,
                StateId = voting.StateId,
                VotingGroups = voting.VotingGroups.ToList(),
                VotingId = voting.VotingId
            };

            return View(detailsVotingView);
        }

        // GET: Votings/Create
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
