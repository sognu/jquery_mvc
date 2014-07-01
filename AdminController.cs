using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;         
using System.Web.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class AdminController : Controller
    {


        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Admin/
        public ActionResult Events()
        {
            List<Event> events = new List<Event>();
            MySqlCommand cmd = new MySqlCommand("selectActiveEvents", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Event e = new Event();
                e.Id = Convert.ToInt32(dr["event"]);
                e.Name = Convert.ToString(dr["name"]);
                e.Description = Convert.ToString(dr["description"]);
                e.Date = Convert.ToDateTime(dr["date"]);
                events.Add(e);
            }
            return View(events);
        }

        // GET: /Admin/
        public ActionResult Users()
        {
            List<User> users = new List<User>();
            MySqlCommand cmd = new MySqlCommand("selectUsers", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                User u = new User();
                u.Id = Convert.ToInt32(dr["id"]);
                u.Event = Convert.ToInt32(dr["event_id"]);
                u.FirstName = Convert.ToString(dr["firstname"]);
                u.LastName = Convert.ToString(dr["lastname"]);
                u.PhoneNumber = Convert.ToString(dr["phonenumber"]);
                u.Admin = Convert.ToByte(dr["admin"]);
                u.UUID = Convert.ToString(dr["uuid"]);
                u.Shift = Convert.ToString(dr["shift"]);
                users.Add(u);
            }
            return View(users);
        }

        // GET: /Admin/
        public ActionResult Groups()
        {
            List<Group> groups = new List<Group>();
            MySqlCommand cmd = new MySqlCommand("selectGroups", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Group g = new Group();
                g.Id = Convert.ToInt32(dr["id"]);
                g.Event = Convert.ToInt32(dr["event_id"]);
                g.Name = Convert.ToString(dr["name"]);
                groups.Add(g);
            }

            return View(groups);
        }

        // GET: /Admin/
        public ActionResult Messages()
        {
            List<Message> messages = new List<Message>();
            MySqlCommand cmd = new MySqlCommand("selectMsgs", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Message m = new Message();
                m.Id = Convert.ToInt32(dr["id"]);
                m.Event = Convert.ToInt32(dr["event_id"]);
                m.Cube = Convert.ToString(dr["systemcube_id"]);
                m.MessageFrom = Convert.ToInt32(dr["messagefrom"]);
                m.MessageTo = Convert.ToInt32(dr["messageto"]);
                m.Group = Convert.ToString(dr["group_id"]);
                m.MessageContent = Convert.ToString(dr["message"]);
                m.SentAt = Convert.ToString(dr["sentat"]);
                m.ReceivedAt = Convert.ToString(dr["receivedat"]);
                m.PhoneFrom = Convert.ToString(dr["phonefrom"]);
                m.PhoneTo = Convert.ToString(dr["phoneto"]);   
                messages.Add(m);
            }
            return View(messages);
        }

        [HttpPost]
        public ActionResult CreateEvent(Event e)
        {

         try { 
                MySqlCommand cmd = new MySqlCommand("insertEvent", new MySqlConnection(GetConnectionString()));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                cmd.Parameters.Add(new MySqlParameter("in_name", e.Name));
                cmd.Parameters.Add(new MySqlParameter("in_description", e.Description));
                cmd.Parameters.Add(new MySqlParameter("in_date", e.Date));
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
         }
         catch (Exception)
         {
             return Json(new { msg = "ERROR : Please make sure fields are not empty." });
         }
         return null;
        }

        [HttpPost]
        public ActionResult UpdateEvent(Event e)
        {
            try
            {

                MySqlCommand cmd = new MySqlCommand("updateEvent", new MySqlConnection(GetConnectionString()));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                cmd.Parameters.Add(new MySqlParameter("in_event", e.Id));
                cmd.Parameters.Add(new MySqlParameter("in_name", e.Name));
                cmd.Parameters.Add(new MySqlParameter("in_description", e.Description));
                cmd.Parameters.Add(new MySqlParameter("in_date", e.Date));
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
         catch (Exception)
         {
             return Json(new { msg = "ERROR : Please make sure fields are not empty." });
         }
         return null;
        }

        [HttpPost]
        public void DeleteEvent(Event e)
        {
            MySqlCommand cmd = new MySqlCommand("deleteEvent", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("event_id", e.Id));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        [HttpPost]
        public void CloseEvent(Event e)
        {
            MySqlCommand cmd = new MySqlCommand("updateEventInactive", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("event_id", e.Id));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        [HttpPost]
        public ActionResult CreateUser(User u)
        {
            try { 

                MySqlCommand cmd = new MySqlCommand("insertUser", new MySqlConnection(GetConnectionString()));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                string sys_id = getSystemID();
                cmd.Parameters.Add(new MySqlParameter("in_eventid", u.Event));
                cmd.Parameters.Add(new MySqlParameter("in_firstname", u.FirstName));
                cmd.Parameters.Add(new MySqlParameter("in_lastname", u.LastName));
                cmd.Parameters.Add(new MySqlParameter("in_phonenumber", u.PhoneNumber));
                cmd.Parameters.Add(new MySqlParameter("in_uuid", sys_id));
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
         catch (Exception) {
                 return Json(new { msg = "ERROR : Please make sure fields are not empty and Event ID exists." });
        }
         return null;
     }


        [HttpPost]
        public ActionResult UpdateUser(User u)
        {
            try
            {

                MySqlCommand cmd = new MySqlCommand("updateUser", new MySqlConnection(GetConnectionString()));
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                cmd.Parameters.Add(new MySqlParameter("in_userid", u.Id));
                cmd.Parameters.Add(new MySqlParameter("in_firstname", u.FirstName));
                cmd.Parameters.Add(new MySqlParameter("in_lastname", u.LastName));
                cmd.Parameters.Add(new MySqlParameter("in_phonenumber", u.PhoneNumber));
                cmd.Parameters.Add(new MySqlParameter("in_admin", u.Admin));
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }catch (Exception){
             return Json(new { msg = "ERROR : Please make sure fields are not empty." });
         }
         return null;
        }

        [HttpPost]
        public ActionResult UpdateUserShift(User u)
        {
            MySqlCommand cmd = new MySqlCommand("updateUserShift", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("in_userid", u.Id));
            cmd.Parameters.Add(new MySqlParameter("in_shift", u.Shift));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return Json(new { msg = "Successfully updated " + u.Id });
        }


        [HttpPost]
        public void DeleteUser(User u)
        {
            MySqlCommand cmd = new MySqlCommand("deleteUser", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("user_id", u.Id));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }


        [HttpPost]
        public ActionResult CreateGroup(Group g)
        {
          try{

            MySqlCommand cmd = new MySqlCommand("insertGroup", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("in_eventid", g.Event));
            cmd.Parameters.Add(new MySqlParameter("in_name", g.Name));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
          }
         catch (Exception)
         {
             return Json(new { msg = "ERROR : Please make sure fields are not empty and Event ID exists." });
         }
         return null;
        }


        [HttpPost]
        public ActionResult UpdateGroup(Group g)
        {
            try { 
            MySqlCommand cmd = new MySqlCommand("updateGroup", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("in_id", g.Id));
            cmd.Parameters.Add(new MySqlParameter("in_eventid", g.Event));
            cmd.Parameters.Add(new MySqlParameter("in_name", g.Name));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            }
            catch (Exception)
            {
                return Json(new { msg = "ERROR : Please make sure fields are not empty." });
            }
            return null;
        }


        [HttpPost]
        public void DeleteGroup(Group g)
        {
            MySqlCommand cmd = new MySqlCommand("deleteGroup", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.Add(new MySqlParameter("group_id", g.Id));
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }



        private string GetConnectionString()
        {
            return "Server='';Database='';Uid='';Pwd='';Convert Zero Datetime=True";

        }

        private string getSystemID()
        {
            MySqlCommand cmd = new MySqlCommand("selectSystemID", new MySqlConnection(GetConnectionString()));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Connection.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            string system_id = "";
            if (dr.Read()) { system_id = Convert.ToString(dr["id"]); }
            return system_id;

        }

    }
}