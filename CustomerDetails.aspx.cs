using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Xml;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SDNPortal
{
    public partial class _CustomerDetails : Page
    {
        private MySqlConnection connection;
        private  int pageloadcount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            connection = new MySqlConnection("Data Source=localhost;uid=root;password=verizon123;database=sdn;");
            if (!IsPostBack)
            {             
                GetCustomerData(Request.QueryString["CustomerNumber"]);
                GetAllPolicies();
                lstRouters_SelectedIndexChanged(sender, e);
                Session.Add("pagehistory", pageloadcount.ToString());
            }

            pageloadcount = Convert.ToInt32(Session["pagehistory"].ToString()) + 1;
            Session.Add("pagehistory", pageloadcount.ToString());

        }
        protected void GetAllPolicies()
        {
            connection.Open();

            string query3 = "SELECT * FROM policy";

            MySqlCommand cmd3 = new MySqlCommand(query3, connection);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
            DataSet ds2 = new DataSet();
            adp.Fill(ds2);

            lstPavbl.DataSource = ds2.Tables[0];
            lstPavbl.DataTextField = "policyname";
            lstPavbl.DataValueField = "idpolicy";
            lstPavbl.DataBind();            
            connection.Close();
            
        }

        protected void GetCustomerData(string custNumber)
        {
            connection.Open();
            string query1 = String.Format("SELECT * FROM customers where customernumber = {0}", custNumber.ToString()) ;
            int custid=0;
            MySqlCommand cmd1 = new MySqlCommand(query1, connection);

            MySqlDataReader dr = cmd1.ExecuteReader();
            if(dr.HasRows)
            {
                dr.Read();
                custid = dr.GetInt32(0);
                txtNumber.Value = dr.GetString(1);
                txtName.Value = dr.GetString(2);
                txtLocation.Value = dr.GetString(3);
            }
            dr.Close();
            Session.Add("custid", custid);
            string query2 = String.Format("SELECT * FROM routers where idrouters in (select router_id from cust_routers where cust_id = {0})", custid);

            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd2);
            DataSet ds1 = new DataSet();
            adp.Fill(ds1);

            lstRouters.DataSource = ds1.Tables[0];
            lstRouters.DataTextField = "routersname";
            lstRouters.DataValueField = "idrouters";
            lstRouters.DataBind();
            lstRouters.SelectedIndex = 0;
        
            connection.Close();

        }

        protected void lstRouters_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {                
                connection.Open();
            
                string query3 = String.Format("SELECT * FROM policy where idpolicy in (select policy_id from rout_policy where router_id = {0})", lstRouters.SelectedValue);

                MySqlCommand cmd3 = new MySqlCommand(query3, connection);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
                DataSet ds2 = new DataSet();
                adp.Fill(ds2);

                lstPolicy.DataSource = ds2.Tables[0];
                lstPolicy.DataTextField = "policyname";
                lstPolicy.DataValueField = "idpolicy";
                lstPolicy.DataBind();
                if(ds2.Tables[0].Rows.Count>0)
                { 
                    lstPolicy.SelectedIndex = 0;
                    connection.Close();
                    lstPolicy_SelectedIndexChanged(sender, e);
                }
                else
                    connection.Close();
            }
            catch(Exception ex)
            {

            }
        }

        protected void lstPolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //connection.Open();

            //string query4 = String.Format("SELECT * FROM policy where idpolicy = {0}", lstPolicy.SelectedValue);

            //MySqlCommand cmd4 = new MySqlCommand(query4, connection);
            //MySqlDataReader dr2 = cmd4.ExecuteReader();
            //dr2.Read();
            //description.Value = dr2.GetString("policydescription");
            //dr2.Close();
            //connection.Close();

            Get_Policy();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            mpePopUp3.Show();
            connection.Open();
            string query = String.Format("SELECT rp.router_id, routersName, routersDescription FROM rout_policy rp join routers r on rp.router_id = r.idrouters join cust_routers cr on r.idrouters = cr.router_id join customers c on cr.cust_id = c.idcustomers where rp.policy_id = {0}", lstPolicy.SelectedItem.Value);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);

            CustomersGrid.DataSource = ds.Tables[0];
            CustomersGrid.DataBind();
            Session.Add("impactedView", ds.Tables[0]);
            connection.Close();            

            //connection.Open();

            //string query5 = String.Format("update policy set policydescription = '{0}' where idpolicy = {1}", description.Value.Replace(@"""",@"\""").Replace(@"'", @"\'"), lstPolicy.SelectedValue);

            //MySqlCommand cmd4 = new MySqlCommand(query5, connection);
            //int updatestatus = cmd4.ExecuteNonQuery();            
            //connection.Close();
            // Push_Policy();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myFunction", "javascript:history.go(-" + Session["pagehistory"].ToString() + ");", true);

            
        }

        protected void Get_Policy()
        {
            //string NodesRequest = GetNodesLink();
            //string PolicyListRequest = GetPolicyList(lstRouters.SelectedItem.Text);
            //string InterfaceListRequest = GetInterfaces(lstRouters.SelectedItem.Text);
            //string InterfaceDetailRequest = GetInterfaceDetail(lstRouters.SelectedItem.Text, "dp0p160p1");

            string strRequestURL = "";
            WebRequest req;
            HttpWebResponse resp;
            var encoding = ASCIIEncoding.ASCII;

            strRequestURL = CreateRequest(lstRouters.SelectedItem.Text, lstPolicy.SelectedItem.Text);
            req = WebRequest.Create(strRequestURL);
            req.Method = "GET";
            //req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:admin"));      -- Incease credential needed uncomment it.      
            resp = req.GetResponse() as HttpWebResponse;
            using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                description.Value = responseText;
            }
        }

        protected void Get_Interfaces()
        {          
            string strRequestURL = "";
            WebRequest req;
            HttpWebResponse resp;
            var encoding = ASCIIEncoding.ASCII;

            strRequestURL = GetInterfaces(lstRouters.SelectedItem.Text);
            req = WebRequest.Create(strRequestURL);
            req.Method = "GET";
            //req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:admin"));      -- Incease credential needed uncomment it.      
            resp = req.GetResponse() as HttpWebResponse;
            using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();

                JObject joResponse = JObject.Parse(responseText);
                JObject ojObject = (JObject)joResponse["interfaces"];
                JArray array = (JArray)ojObject["vyatta-interfaces-dataplane:dataplane"];
                lstInterfaces.Items.Clear();
                for (int i = 0; i < array.Count; i++)
                {
                    lstInterfaces.Items.Add(array[i]["tagnode"].ToString());
                }
                txtRouterDescr.Value = "";
            }
        }

        protected void AssignPolicytoInterfaces()
        {
            string strRequestURL = "";
            WebRequest req;
            HttpWebResponse resp;
            var encoding = ASCIIEncoding.ASCII;
            string pushinterface = "{\"vyatta-interfaces-dataplane:dataplane\":[{\"tagnode\": ";

            strRequestURL = GetInterfaceDetail(txtRouterName.Value, lstInterfaces.SelectedItem.Value);
            req = WebRequest.Create(strRequestURL);
            req.Method = "GET";            
            resp = req.GetResponse() as HttpWebResponse;
            using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                JObject joResponse = JObject.Parse(responseText);                
                JArray array = (JArray)joResponse["vyatta-interfaces-dataplane:dataplane"];                
                pushinterface = pushinterface + "\"" + array[0]["tagnode"].ToString() + "\", \"vyatta-policy-qos:qos-policy\": ";
                pushinterface = pushinterface + "\"" + lstPmap.SelectedItem.Text + "\", \"address\": ";
                pushinterface = pushinterface + array[0]["address"].ToString() + "}]}";

            }
                       
            req = WebRequest.Create(strRequestURL);
            req.Method = "PUT";
            req.ContentType = "application/json";

            encoding = new UTF8Encoding();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(pushinterface);
            req.ContentLength = bytes.Length;

            using (var writeStream = req.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }
        }

        protected void Push_Policy(string action,string routername, string policyname, string policydescr)
        {           
            string strRequestURL = "";
            WebRequest req;                

            if (action == "AddPolicy")
            {
                strRequestURL = CreateRequest(routername, policyname); 
                req = WebRequest.Create(strRequestURL);
                req.Method = "PUT";
                req.ContentType = "application/json";

                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(policydescr);
                req.ContentLength = bytes.Length;

                using (var writeStream = req.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }   
            else if(action == "DeletePolicyMap")
            {
                strRequestURL = CreateRequest(routername, policyname);
                req = WebRequest.Create(strRequestURL);
                req.Method = "DELETE";
               // req.ContentType = "application/json";
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;                
            }                    
        }

        protected void btnAPI_Click(object sender, EventArgs e)
        {

            // Test REST API access
            string PolicyRequest = CreateRequest(lstRouters.SelectedItem.Text, lstPolicy.SelectedItem.Text);
            string NodesRequest = GetNodesLink();
            string PolicyListRequest = GetPolicyList(lstRouters.SelectedItem.Text);
            string InterfaceListRequest = GetInterfaces(lstRouters.SelectedItem.Text);
            string InterfaceDetailRequest = GetInterfaceDetail(lstRouters.SelectedItem.Text, "dp0p160p1");
          
            WebRequest req = WebRequest.Create(InterfaceDetailRequest);
            req.Method = "GET";
            //req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:admin"));            
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                description.Value = responseText;
            }          

        }

        protected string CreateRequest(string routername, string policyname)
        {
            string UrlRequest = ConfigurationManager.AppSettings["APILink"].ToString() + routername +
                "/yang-ext:mount/vyatta-policy:policy/vyatta-policy-qos:qos/" + policyname + "/";
                                 
            return (UrlRequest);
        }

        protected string GetNodesLink()
        {
            string UrlRequest = ConfigurationManager.AppSettings["APILink"].ToString();
            return (UrlRequest);
        }

        protected string GetPolicyList(string routername)
        {
            string UrlRequest = ConfigurationManager.AppSettings["APILink"].ToString() + routername + "/yang-ext:mount/vyatta-policy:policy/";
            return (UrlRequest);
        }

        protected string GetInterfaces(string routername)
        {
            string UrlRequest = ConfigurationManager.AppSettings["APILink"].ToString() + routername + "/yang-ext:mount/vyatta-interfaces:interfaces";
            return (UrlRequest);
        }

        protected string GetInterfaceDetail( string routername, string interfacename)
        {
            string UrlRequest = ConfigurationManager.AppSettings["APILink"].ToString() + routername + "/yang-ext:mount/vyatta-interfaces:interfaces/vyatta-interfaces-dataplane:dataplane/" + interfacename;
            return (UrlRequest);
        }      
               
        protected void btnPopup_Click(object sender, EventArgs e)
        {
            mpePopUp.Show();
        }

        protected void routSave_Click(object sender, EventArgs e)
        {
            if(hidRType.Value=="New")
            { 
                connection.Open();

                string squery = String.Format("insert into routers select max(idrouters)+1, '{0}','{1}' from routers", txtRouterName.Value,txtRouterDescr.Value);
                MySqlCommand cmd1 = new MySqlCommand(squery, connection);
                int updatestatus = cmd1.ExecuteNonQuery();

                squery = "SELECT max(idrouters) FROM routers";
                MySqlCommand cmd2 = new MySqlCommand(squery, connection);
                MySqlDataReader dr = cmd2.ExecuteReader();
                dr.Read();
                string routid = dr.GetString(0);
                dr.Close();

                foreach(ListItem l1 in lstPmap.Items)
                { 
                    squery = String.Format("insert into rout_policy select max(idrout_policy)+1, {0},{1} from rout_policy", routid, l1.Value);
                    MySqlCommand cmd3 = new MySqlCommand(squery, connection);
                    updatestatus = cmd3.ExecuteNonQuery();
                }

                squery = String.Format("insert into cust_routers select max(idcust_routers)+1, {0},{1} from cust_routers", Session["custid"].ToString(), routid);
                MySqlCommand cmd4 = new MySqlCommand(squery, connection);
                updatestatus = cmd4.ExecuteNonQuery();

                connection.Close();
                GetCustomerData(Request.QueryString["CustomerNumber"]);
                mpePopUp.Hide();
            }
            else
            {                
                connection.Open();
                string routid = lstRouters.SelectedItem.Value;
                string squery = String.Format("update routers set routersdescription = '{0}' where idrouters = {1}", txtRouterDescr, routid);
                MySqlCommand cmd1 = new MySqlCommand(squery, connection);
                int updatestatus = cmd1.ExecuteNonQuery();

                squery = String.Format("delete from rout_policy where router_id = {0}",routid);
                MySqlCommand cmd2 = new MySqlCommand(squery, connection);
                cmd2.ExecuteNonQuery();

                foreach(ListItem lp in lstPolicy.Items)
                { 
                    try
                    { 
                        Push_Policy("DeletePolicyMap", lstRouters.SelectedItem.Text, lstPolicy.SelectedItem.Text, "");
                    }
                    catch { }
                }
                foreach (ListItem l1 in lstPmap.Items)
                {
                    squery = String.Format("insert into rout_policy select case when max(idrout_policy) is null then 1 else max(idrout_policy)+1 end, {0},{1} from rout_policy", routid, l1.Value);
                    MySqlCommand cmd3 = new MySqlCommand(squery, connection);
                    updatestatus = cmd3.ExecuteNonQuery();

                    squery = "SELECT policyname,policydescription FROM policy where idpolicy = " + l1.Value;
                    MySqlCommand cmdpol = new MySqlCommand(squery, connection);
                    MySqlDataReader dr = cmdpol.ExecuteReader();
                    dr.Read();
                    Push_Policy("AddPolicy", lstRouters.SelectedItem.Text, dr.GetString(0), dr.GetString(1));
                    dr.Close();
                }                          
                connection.Close();
                
                GetCustomerData(Request.QueryString["CustomerNumber"]);
                mpePopUp.Hide();
            }
        }

        protected void routCancel_Click(object sender, EventArgs e)
        {
            mpePopUp.Hide();
        }

                
        protected void PMap_Click(object sender, EventArgs e)
        {
            foreach(int li in lstPavbl.GetSelectedIndices())
            {
                ListItem avl = new ListItem(lstPavbl.Items[li].Text, lstPavbl.Items[li].Value);
                lstPmap.Items.Add(avl);
            }
            mpePopUp.Show();
        }
        protected void PuMap_Click(object sender, EventArgs e)
        {
            foreach (int li in lstPmap.GetSelectedIndices())
            {
                lstPmap.Items.Remove(lstPmap.Items[li]);
            }
            mpePopUp.Show();
        }

        protected void lnkRouterAdd_Click(object sender, EventArgs e)
        {
            mpePopUp.Show();
        }

        protected void lnkPolicyAdd_Click(object sender, EventArgs e)
        {
            hidimpact.Value = "No";
            mpePopUp2.Show();
        }
                
        protected void polyCancel_Click(object sender, EventArgs e)
        {

        }

        protected void polySave_Click(object sender, EventArgs e)
        {
            connection.Open();

            string squery = String.Format("insert into policy select max(idpolicy)+1, '{0}','{1}' from policy", txtNewPloicyName.Value, txtNewPolicyDescr.Value);
            MySqlCommand cmd1 = new MySqlCommand(squery, connection);
            int updatestatus = cmd1.ExecuteNonQuery();

            squery = "SELECT max(idpolicy) FROM policy";
            MySqlCommand cmd2 = new MySqlCommand(squery, connection);
            MySqlDataReader dr = cmd2.ExecuteReader();
            dr.Read();
            string polyid = dr.GetString(0);
            dr.Close();
            if (hidimpact.Value == "No")
            {
                squery = String.Format("insert into rout_policy select max(idrout_policy)+1, {0},{1} from rout_policy", lstRouters.SelectedItem.Value, polyid);
                MySqlCommand cmd3 = new MySqlCommand(squery, connection);
                updatestatus = cmd3.ExecuteNonQuery();
                Push_Policy("AddPolicy", lstRouters.SelectedItem.Text, txtNewPloicyName.Value, txtNewPolicyDescr.Value);
            }
            else
            {
                DataTable dt = (DataTable)Session["impactedView"];
                int irows = 0;
                foreach (GridViewRow row in CustomersGrid.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chksel");                   
                    
                    if (cb != null && cb.Checked)
                    {
                        squery = String.Format("insert into rout_policy select max(idrout_policy)+1, {0},{1} from rout_policy", dt.Rows[irows]["router_id"].ToString() , polyid);
                        MySqlCommand cmd3 = new MySqlCommand(squery, connection);
                        updatestatus = cmd3.ExecuteNonQuery();
                        Push_Policy("AddPolicy", dt.Rows[irows]["routersName"].ToString(), txtNewPloicyName.Value, txtNewPolicyDescr.Value);

                        squery = String.Format("delete from rout_policy where router_id = {0} and policy_id = {1}", dt.Rows[irows]["router_id"].ToString(), lstPolicy.SelectedValue);
                        cmd3 = new MySqlCommand(squery, connection);
                        updatestatus = cmd3.ExecuteNonQuery();
                        Push_Policy("DeletePolicyMap", dt.Rows[irows]["routersName"].ToString(), txtNewPloicyName.Value, txtNewPolicyDescr.Value);
                        //need to call API to delete policy

                    }
                    irows += 1;

                }
            }
            connection.Close();

            GetCustomerData(Request.QueryString["CustomerNumber"]);
            GetAllPolicies();
            mpePopUp.Hide();
        }

        protected void chkselh_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbh = (CheckBox)CustomersGrid.HeaderRow.FindControl("chkselh");
            // Iterate through the Products.Rows property
            foreach (GridViewRow row in CustomersGrid.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl("chksel");

                if (cb != null)
                    cb.Checked = cbh.Checked;
            }
            mpePopUp3.Show();
        }

        protected void btnCancelP_Click(object sender, EventArgs e)
        {
            mpePopUp3.Hide();
        }

        protected void btnUpdatePolicy_Click(object sender, EventArgs e)
        {
            int chkcount = 0;
            foreach (GridViewRow row in CustomersGrid.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl("chksel");

                if (cb != null && cb.Checked)
                {
                    chkcount += 1;
                }
                    
            }
            if(CustomersGrid.Rows.Count == chkcount)
            {
                connection.Open();

                string query5 = String.Format("update policy set policydescription = '{0}' where idpolicy = {1}", description.Value.Replace(@"""", @"\""").Replace(@"'", @"\'"), lstPolicy.SelectedValue);

                MySqlCommand cmd4 = new MySqlCommand(query5, connection);
                int updatestatus = cmd4.ExecuteNonQuery();
                Push_Policy("AddPolicy", lstRouters.SelectedItem.Text , lstPolicy.SelectedItem.Text, description.Value);

                connection.Close();
            }
            else
            {
                hidimpact.Value = "Yes";
                txtNewPloicyName.Value = "Copy of " + lstPolicy.SelectedItem.Text;
                txtNewPolicyDescr.Value = description.Value;
                mpePopUp2.Show();
            }

            mpePopUp3.Hide();
        }

        protected void lnkEditRout_Click(object sender, EventArgs e)
        {
            mpePopUp.Show();
            txtRouterName.Value = lstRouters.SelectedItem.Text;
            //txtRouterName.Disabled = true;
            Get_Interfaces();
            lstPmap.Items.Clear();
            foreach(ListItem li in lstPolicy.Items)
            {
                lstPmap.Items.Add(li);
            }

        }

        protected void routMapInterface_Click(object sender, EventArgs e)
        {
            AssignPolicytoInterfaces();
            lblpopRmessage.Text = "Selected interface mapped with selected policy";
            mpePopUp.Show();
        }
    }
}