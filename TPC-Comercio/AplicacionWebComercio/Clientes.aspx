<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="AplicacionWebComercio.Clientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  


    <h2>Listado de Clientes</h2>

     <div class ="row">
     <div class="col-6">
        <div class="mb-3">
            <asp:Label Text="Filtrar" runat="server" />
            <asp:TextBox runat= "server" ID="filtro" AutoPostBack="true" OnTextChanged="filtro_TextChanged" CssClass="form-control mb-3" />
      </div> 
          </div>

      </div>

            <div class="col-6" style= "display: flex; flex-direction: column; justify-content: flex-end;" >
            <div class="mb-3" > 
            <asp:CheckBox ID="chkAvanzado" runat="server" Text="Filtro Avanzado" CssClass="form-check-input" AutoPostBack="true" OnCheckedChanged="chkAvanzado_CheckedChanged" />
                </div> 

                    </div>
            
       
    <%if (chkAvanzado.Checked)
        { %>
    <div class ="row">
        <div class="col-3">
            <div class="mb-3">
                <asp:Label Text="Campo" ID="lblCampo" runat="server" />
                <asp:DropDownList runat="server" AutoPostBack="true" ID="ddlCampo" CssClass="form-control" OnSelectedIndexChanged="ddlCampo_SelectedIndexChanged">
                        <asp:ListItem Text="Seleccione..." Value="" />
                        <asp:ListItem Text="Nombre" />
                        <asp:ListItem Text="Apellido" />
                        <asp:ListItem Text="DNI" />
                </asp:DropDownList>
            </div>
            </div>
        </div>  
             <div class="col-3">
            <div class="mb-3">
                <asp:Label Text="Criterio"  runat="server" />
                <asp:DropDownList runat="server" ID="ddlCriterio" CssClass="form-control">
                <asp:ListItem Text="Seleccione..." Value="" />
                    </asp:DropDownList>
            </div>
        </div>
    
           <div class="col-3">
           <div class="mb-3">
                <asp:Label Text="Filtro" runat="server" />
                <asp:TextBox runat="server" ID="txtFiltroAvanzado" CssClass="form-control"></asp:TextBox>
           </div>
               </div>
            <div class="col-3">
             <div class="mb-3">
                <asp:Label Text="Estado" runat="server" />
                <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control">
                        <asp:ListItem Text="Todos" />
                        <asp:ListItem Text="Activo" />
                        <asp:ListItem Text="Inactivo" /> 
                </asp:DropDownList>
        </div>

        </div>
  
               
                <div class ="row">
                    <div class ="col-3">    
                        <div class="mb-3">
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                            
                        </div>

                    </div>
                    </div>
    <% } %> 

     <asp:Button ID="btnAgregar" runat="server" Text="Agregar Cliente"  CssClass="btn btn-success mb-3" PostBackUrl="~/ClientesFormularios.aspx" />
    
    <br /><br />     
    
    <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="true" CssClass="table"
        AutoGenerateSelectButton="true" OnSelectedIndexChanged="dgvClientes_SelectedIndexChanged" OnRowCreated="dgvClientes_RowCreated"> 
     
    </asp:GridView> 



</asp:Content>
