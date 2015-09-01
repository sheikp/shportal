<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerDetails.aspx.cs" Inherits="SDNPortal._CustomerDetails" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-wrapper">
        <section id="main" class="container">
					<h2>SDN Governance</h2>					
            <div class="row">
						<div class="12u">
                            <section class="box">
                                
					                <div class="row uniform 50%">
                                        <div class="4u 12u(mobilep)">
												<h4>Customer Number</h4>
											</div>
											<div class="4u 12u(mobilep)">
												<h4>Customer Name</h4>
											</div>
                                            <div class="4u 12u(mobilep)">
												<h4>Location</h4>
											</div>											
										</div>  
                                <div class="row uniform 50%">
                                        <div class="4u 12u(mobilep)">
												<input runat="server" type="text" name="query" id="txtNumber" value="" placeholder="Customer Number" readonly="readonly" />
											</div>
											<div class="4u 12u(mobilep)">
												<input runat="server" type="text" name="query" id="txtName" value="" placeholder="Customer Name" readonly="readonly" />
											</div>
                                            <div class="4u 12u(mobilep)">
												<input runat="server" type="text" name="query" id="txtLocation" value="" placeholder="Location" readonly="readonly" />
											</div>											
										</div>  
                                <div class="row uniform 50%">
                                        <div class="6u">
												<h4>List of Routers</h4>
											</div>                                 
                                        <div class="6u">
												<h4>Policies on selected Router</h4>
										</div>            
                                     </div>        
                                <div class="row uniform 50%">
                                        <div class="6u">
												<div class="select-wrapper">
													<asp:ListBox ID="lstRouters" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstRouters_SelectedIndexChanged"></asp:ListBox>
												</div>
											</div>
                                 
                                    <div class="6u">
												<div class="select-wrapper">
													<asp:ListBox ID="lstPolicy" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstPolicy_SelectedIndexChanged"></asp:ListBox>
												</div>
											</div>            
                                     </div>          
                                 <div class="row uniform 50%">
                                        <div class="12u">
												<div class="select-wrapper">
													<textarea name="message" id="description" placeholder="Policy Description" rows="6" runat="server"></textarea>
												</div>
											</div>
                                     </div> 
                                 <div class="row uniform">
						                <div class="12u">
							                <ul class="actions">
								                <li><asp:Button runat="server" ID="btnSave" Text="Save Policy Detail" OnClick="btnSave_Click" /></li>
								                <li><asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" CssClass="alt" /></li>
                                                <li><asp:Button runat="server" ID="Button1" Text="REST API TEST" OnClick="btnAPI_Click" CssClass="alt" /></li>
							                </ul>
						                </div>
					                </div>
                                </section>
						</div>
					</div>                  
                           
		</section>
    </div>

</asp:Content>
