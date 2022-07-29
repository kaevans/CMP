$(function () {
    $(this).css("background-color", "lightgray");
    $("#dropDownListdiv").on("change", "select", function () {
        var value = $(this).val();
        var id = $(this).attr("id");
        $.post("PipelineDeployment/setDropDrownList", { type: id, value: value }, function (data) {
            console.log("id:" + id + ", Data: " + data + ", Subscriptions: " + data.subscriptions + ", ResourceGroups: " + data.resourceGroups);
            switch (id) {
                case "SubscriptionId":
                    PopulateDropDown("#ResourceGroupId", data.resourceGroups);
                    break;
            }
        });
    });
});
function PopulateDropDown(dropDownId, list) {
    $(dropDownId).empty();
    console.log("Dropdown: " + dropDownId + ", List: " + list);
    
    $(dropDownId).append("<option>Please select</option>")
    $.each(list, function (index, row) {
        console.log("Index:" + index + ", Row Value: " + row.value + ", Row Text: " + row.text);
        if (index == 0) {            
            $(dropDownId).append("<option value='" + row.value + "' selected='selected'>" + row.text + "</option>");
        } else {
            $(dropDownId).append("<option value='" + row.value + "'>" + row.text + "</option>")
        }
    });
}