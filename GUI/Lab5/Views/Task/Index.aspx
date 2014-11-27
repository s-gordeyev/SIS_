<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<TaskLib.Task>>" %>
<%@ Import namespace="TaskLib" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="form1" action="/" method="post" runat="server">

    <h2>Tasks</h2>
    <div style="overflow:auto">
    <table>
        <tr>
            <th>
                ID
            </th>
            <th>
                URL
            </th>
            <th>
                Destination
            </th>

            <th>
                Result
            </th>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <input type="text" name="CreateTask" id="tn"/>
            </td>
            <td>
                <input type="text" name="CreateTask" id="td"/>
            </td>
            <td>
                 <input type="button" value="Create" onclick="document.forms[0].action = 'Task/Create';document.forms[0].submit();"/>
            </td>
        </tr>

    <% foreach (Task item in Model)
       { %>
    
        <tr><td><%: item.id %></td><td><%: item.url %></td></td><td><%: ((TaskLib.TaskArchiveSite)item).destination %></td><td><%
           if (item.result == null) { %>
                <input type="button" onclick="location.reload()" value="Refresh"/> 
           <%}
           else if (item.result.ToString().Length > 10) {%> 
               <input type="button" onclick="location.assign('Task/TaskResult?id=<%: item.id %>')" value="Show Result"/>
           <%}
           else {
                   Writer.Write(item.result.ToString());
           }%></td>
        </tr>
    
    <% } %>

    </table>
    </div>

</form>

</asp:Content>

