/***************************************************************************
   Copyright 2015 OSIsoft, LLC.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 ***************************************************************************/

var baseUrl = "https://myserver/piwebapi";
var piServer = "MYPIDataArchive";
var piTagWebIds = {};

// Plot options
var options = {
    xaxis: {
        mode: "time",
        timeformat: "%Y/%m/%d %H:%M:%S"
    }
};

// Get base64-encoded string for basic authentication
var makeBasicAuth = function (user, password) {
    var tok = user + ':' + password;
    var hash = window.btoa(tok);
    return "Basic " + hash;
};

// Ajax get request with basic authentication
var getAjax = function (url, successCallBack) {
    $.ajax({
        url: encodeURI(url),
        method: 'GET',
        success: successCallBack,
        error: function (xhr) {
            console.log(xhr.responseText);
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', makeBasicAuth('username', 'password'));
        }
    });
}

$(document).ready(function () {
    // Getting the Web Id for PI Data Archive
    var piServerWebId;
    var url = baseUrl + "/dataservers?name=" + piServer;
    var ajax = getAjax(url, function (data) {
        piServerWebId = data.WebId;
        $('#search-btn').removeAttr('disabled');
    });

    // Getting the list of tags defined in tag mask
    $('#search-btn').click(function (data) {
        $('#tag-select').empty();
        var tagMask = $('#tagmask-text').val();
        if (tagMask.length > 0) {
            var url = baseUrl + "/dataservers/" + piServerWebId + "/points?nameFilter=" + $('#tagmask-text').val();
            getAjax(url, function (data) {
                for (var i = 0; i < data.Items.length; i++) {
                    $('#tag-select').append("<option value=\"" + data.Items[i].Name + "\">" + data.Items[i].Name + "</option>");
                    piTagWebIds[data.Items[i].Name] = data.Items[i].WebId;
                }
                $('#plot-btn').removeAttr('disabled');
            });
        } else {
            $('#plot-btn').attr('disabled', 'disabled');
        }
    });

    // Plotting the selected tag between start and end time
    $('#plot-btn').click(function () {
        var tag = $('#tag-select option:selected').text();
        var startTime = $('#start-time-text').val();
        var endTime = $('#end-time-text').val();

        if (tag.length > 0) {
            var url = baseUrl + "/streams/" + piTagWebIds[tag] + "/plot?intervals=800";
            if (startTime.length > 0) {
                url += "&starttime=" + startTime;
            }
            if (endTime.length > 0) {
                url += "&endtime=" + endTime;
            }
            getAjax(url, function (data) {
                var plotData = [[]];
                for (var i = 0; i < data.Items.length; i++) {
                    if (data.Items[i].Good) {
                        plotData[0].push([new Date(data.Items[i].Timestamp).getTime(), data.Items[i].Value]);
                    }
                }

                $.plot($('#plot'), plotData, options);
            });
        }
        return false;
    });
});