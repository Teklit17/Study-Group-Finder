$(document).ready(function () {
    // Toggle for group form
    document.getElementById("createStudyGroupButton").onclick = function () {
        var form = document.getElementById("createStudyGroupForm");
        form.style.display = form.style.display === "none" ? "block" : "none";
    };

    // Form submission (ajax)
    $("#createStudyGroupFormId").submit(function (event) {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: '/StudyGroup/Create',
            data: $(this).serialize(),
            success: function (response) {
                $('#groupList').append(`
                    <li>
                        <span class="group-name" style="cursor:pointer;" data-id="${response.id}">${response.groupName}</span>
                        <div class="group-details" style="display:none;">
                            <p>${response.groupDescription}</p>
                            <p><strong>Group Members</strong> (Max: ${response.maxGroupMembers})<strong>:</strong></p>
                            <ul>
                            <li>${response.creatorUserName} (Creator)</li>
                            </ul>
                            <button class="DeleteGroupButton" data-id="${response.id}">Delete Group</button>
                        </div>
                    </li>
                `);

                $('#createStudyGroupFormId')[0].reset();
            },
            error: function (xhr) {
                alert("Error creating group: " + xhr.responseText);
            }
        });
    });

    // Toggle group details
    $(document).on("click", ".group-name", function () {
        var details = $(this).next(".group-details");
        details.toggle();
    });

    // Join group button
    $(document).on("click", ".JoinGroupButton", function () {
        var groupId = $(this).data("id");
        var button = $(this);

        $.ajax({
            type: "POST",
            url: `/StudyGroup/Join/${groupId}`,
            success: function (response) {
                alert("You have joined the Study Group!");
                button.siblings("ul").append(`<li>${response.userName}</li>`);
            },
            error: function (xhr) {
                alert("Error joining group: " + xhr.responseText);
            }
        });
    });

    // Delete group button
    $(document).on("click", ".DeleteGroupButton", function () {
        var groupId = $(this).data("id");

        if (confirm("Are you sure you want to delete this group?")) {
            $.ajax({
                type: "POST",
                url: `/StudyGroup/Delete/${groupId}`,
                success: function (response) {
                    if (response.success) {
                        $(`.DeleteGroupButton[data-id='${groupId}']`).closest("li").remove();
                        alert("Group deleted successfully.");
                    }
                },
                error: function (xhr) {
                    alert("Error deleting group: " + xhr.responseText);
                }
            });
        }
    });

    // Search groups
    $("#searchInput").on("input", function () {
        var query = $(this).val();

        if (query === "") {
            $.ajax({
                url: '/StudyGroup/Index',
                type: 'GET',
                success: function (response) {
                    $("#searchResults").html($(response).find("#searchResults").html());
                },
                error: function (xhr) {
                    console.error("Failed to reload groups:", xhr.responseText);
                }
            });
        } else {
            $.ajax({
                url: '/StudyGroup/Search',
                type: 'GET',
                data: { query: query },
                success: function (response) {
                    $("#searchResults").html(response);
                },
                error: function (xhr) {
                    console.error("Search failed:", xhr.responseText);
                }
            });
        }
    });
});


