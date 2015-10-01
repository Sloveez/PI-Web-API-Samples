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

// Define the names and object types of the AF hierarchy
var childrenMap = {
    PISystems: ['AssetServers'],
    AssetServers: ['Databases'],
    Databases: ['Elements'],
    Elements: ['Elements', 'Attributes'],
    Attributes: ['Attributes']
};

// Constructor for the node object
function node(name, type, links, parentDiv) {
    this.type = type;
    this.links = links;
    this.flipper = $('<span class="flipper">+</span>').click(flip.bind(this, this));
    parentDiv.append(this.flipper).append('<span class="' + type + '"> ' + name + '</span><br />');
    this.div = $('<div></div>').hide().appendTo(parentDiv);
}

// Make ajax calls to PI Web API server to get the children of each node
function loadChildren(n) {
    n.loaded = true;
    childrenMap[n.type].forEach(function (childCollection) {
        $.ajax({
            url: n.links[childCollection],
            success: function (collection) {
                n[childCollection] = collection.Items.map(function (item) {
                    return new node(item.Name, childCollection, item.Links, n.div);
                })
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Basic xxx');
            }
        })
    });
}

// Load children, toggle the flipper icon and show/hide the loaded element
function flip(n) {
    if (!n.loaded) { loadChildren(n); }
    n.flipper.html(n.flipper.html() == '+' ? '-' : '+');
    n.div.toggle();
}

// Add the root node when the DOM is fully loaded
$(function () {
    root = new node('PI Systems', 'PISystems',
      { AssetServers: 'https://myserver/piwebapi/assetservers' }, $("#root"));
});