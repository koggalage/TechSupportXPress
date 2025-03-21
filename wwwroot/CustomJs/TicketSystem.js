﻿"use strict";
// Preloader
$(window).on("load", function () {
    //console.log("system js on load");
    $(".loader").fadeOut("fast");
    $("#spin").remove();
});
$(document).on("change", "#CategoryId.get-subcategories", function (e) {
    //console.log("system js on change");


    var working = "<span id ='spin' ><i class='fa fa-spinner fa-spin fa-3x fa-fw'></i></span>";
    if ($(this).val() !== "") {
        $.ajax({
            url: "/Data/GetTicketSubCategories/" + $(this).val(),
            dataType: "json",
            crossDomain: true,
            beforeSend: function () {
                $(this).parent().append(working);
                $("#SubCategoryId").html("");
                $("#SubCategoryId").append("<option value> --Select Sub-Categories -- </option>");
            },
            success: function (json) {
                var data = json
                console.log(data);
                $(data).map(function () {
                    $("#SubCategoryId").append($('<option></option>').val(this.id).html(this.name));
                    $("#spin").remove();
                });
            },
            error: function (xhr, ajaxOptions, thrownError) {
            },
            complete: function () {
                $("#spin").remove();
            }
        });
    }
});