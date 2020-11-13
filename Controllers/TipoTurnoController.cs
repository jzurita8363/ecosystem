﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ecosystem.Controllers;
using ecosystem.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using SimpleCrypto;
using System.Net.Mail;
using System.Net;

namespace ecosystem.Controllers
{
    public class TipoTurnoController : Controller
    {
        public string bd = "radiotaxi.";
        public string conexion = "Data Source=Desarrollo-pc; port = 3306; Initial Catalog = radiotaxi; User Id = root;password = smartdicijj";

        public ActionResult TipoTurno()
        {
            if (System.Web.HttpContext.Current.Session["sessionClosed"] == null)
            {
                
                ViewBag.Usuario = System.Web.HttpContext.Current.Session["sessionString"];
                ViewBag.Perfil = System.Web.HttpContext.Current.Session["perfil"];
                ViewBag.Correo = System.Web.HttpContext.Current.Session["correo"];
                ViewBag.ConductoresConf = System.Web.HttpContext.Current.Session["conductoresConf"];
                return View();
            }
            else
            {
                return RedirectToAction("../Login/Login");
            }
        }
        public String ListaTipoTurno()//crear JSON TipoLicencia
        {
            string cadena = "{\"data\":[";
            int i = 0;
            string constr = conexion;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "select JSON_OBJECT('Cod_Turno', Cod_Turno, 'Nom_Turno', Nom_Turno, 'Hora_Inicio', Hora_Inicio, 'Hora_Termino', Hora_Termino) AS json_string from web_vtab_tip_Turnos";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            i++;
                        }
                    }
                    con.Close();
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            if (i == 1)
                            {
                                cadena += Convert.ToString(sdr["json_string"]);
                            }
                            else
                            {
                                cadena += Convert.ToString(sdr["json_string"]) + ",";
                            }
                        }
                        cadena += "]}";
                        cadena = cadena.Replace(",]}", "]}");
                    }
                    con.Close();
                }
            }
            return cadena;
        }

        //GUARDAR TIPO TURNO
        public JsonResult GuardarTipoTurno(List<TipoTurno> dataToProcess)
        {
            TipoTurno Movil = new TipoTurno();
            foreach (var item in dataToProcess)
            {
                Movil.Cod_Turno = item.Cod_Turno;
                Movil.Nom_Turno = item.Nom_Turno;
                Movil.Hora_Inicio = item.Hora_Inicio;
                Movil.Hora_Termino = item.Hora_Termino;
                Movil.User_Log = item.User_Log;

                //LLENADO DE BD
                string constr = conexion;
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(bd + "web_pgraba_tip_turnos", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_Cod_Turno", Movil.Cod_Turno);
                    cmd.Parameters.AddWithValue("p_Nom_Turno", Movil.Nom_Turno);
                    cmd.Parameters.AddWithValue("p_Hora_Inicio", Movil.Hora_Inicio);
                    cmd.Parameters.AddWithValue("p_Hora_Termino", Movil.Hora_Termino);
                    cmd.Parameters.AddWithValue("p_User_Log", Movil.User_Log);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Json(dataToProcess, JsonRequestBehavior.AllowGet);
        }

        // ACTUALIZAR TIPO TURNO
        public JsonResult ActualizarTipoTurno(List<TipoTurno> dataToProcess)
        {
            TipoTurno Movil = new TipoTurno();
            foreach (var item in dataToProcess)
            {
                Movil.Cod_Turno = item.Cod_Turno;
                Movil.Nom_Turno = item.Nom_Turno;
                Movil.Hora_Inicio = item.Hora_Inicio;
                Movil.Hora_Termino = item.Hora_Termino;
                Movil.User_Log = item.User_Log;

                //LLENADO DE BD
                string constr = conexion;
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(bd + "web_pactualiza_turnos", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_Cod_Turno", Movil.Cod_Turno);
                    cmd.Parameters.AddWithValue("p_Nom_Turno", Movil.Nom_Turno);
                    cmd.Parameters.AddWithValue("p_Hora_Inicio", Movil.Hora_Inicio);
                    cmd.Parameters.AddWithValue("p_Hora_Termino", Movil.Hora_Termino);
                    cmd.Parameters.AddWithValue("p_User_Log", Movil.User_Log);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Json(dataToProcess, JsonRequestBehavior.AllowGet);
        }

        // EDITAR TIPO TURNOS
        public String editTipoTurno(string Cod_Turno)//crear JSON Turnos
        {
            string cadena = "";
            string constr = conexion;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "select JSON_OBJECT('Cod_Turno', Cod_Turno, 'Nom_Turno', Nom_Turno, 'Hora_Inicio', Hora_Inicio, 'Hora_Termino', Hora_Termino) AS json_string from web_vtab_tip_Turnos WHERE Cod_Turno='" + Cod_Turno + "'";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cadena = Convert.ToString(sdr["json_string"]);
                        }
                    }
                    con.Close();
                }
            }
            return cadena;
        }

        // ELIMINAR TIPO TURNO
        public String EliminarTipoTurno(string Cod_Turno, string User_Log) //crear JSON para eliminar y traer mensaje
        {
            string cadena = "";
            string constr = conexion;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string query = "call web_pelimina_tip_turnos ('" + Cod_Turno + "', '" + User_Log + "' )";
                using (MySqlCommand cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cadena = Convert.ToString(sdr["Json_String"]);
                        }
                    }
                    con.Close();
                }
            }
            return cadena;
        }

    }
}
