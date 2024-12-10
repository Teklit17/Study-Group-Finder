$(document).ready(function () {
  // Toggle for group form
  document.getElementById("createStudyGroupButton").onclick = function () {
    const form = document.getElementById("createStudyGroupForm");
    form.style.display = form.style.display === "none" ? "block" : "none";
  };

  // Form submission (ajax)
  $("#createStudyGroupFormId").submit(function (event) {
    event.preventDefault();
    $.ajax({
      type: "POST",
      url: "/StudyGroup/Create",
      data: $(this).serialize(),
      success: function (response) {
        const currentUserId = $("#currentUserId").val();
        const isCreator = response.creatorId === currentUserId;
        
        $("#searchResults .row").append(
            `<div class="col-md-4 mb-4">
              <div class="card">
                <div class="card-body">
                  <h5 class="card-title group-name" style="cursor:pointer;" data-id="${response.id}">
                    ${response.groupName}
                  </h5>
                  <div class="group-details" style="display:none;">
                    <p class="card-text">${response.groupDescription}</p>
                    <p><strong>Group Members</strong> (Max: ${response.maxGroupMembers})<strong>:</strong></p>
                    <ul>
                      <li>${response.creatorUserName} (Creator)</li>
                    </ul>
                    ${!isCreator ? `<button class="btn btn-primary JoinGroupButton" data-id="${response.id}">
                      Join ${response.groupName}
                    </button>` : ''}
                    ${isCreator ? `<button class="btn btn-danger DeleteGroupButton" data-id="${response.id}">
                      Delete Group
                    </button>` : ''}
                  </div>
                </div>
              </div>
            </div>`
        );

        $("#createStudyGroupFormId")[0].reset();
        $("#createStudyGroupForm").hide();
      },
      error: function (xhr) {
        alert("Error creating group: " + xhr.responseText);
      },
    });
  });

  // Toggle group details
  $(document).on("click", ".group-name", function () {
    const details = $(this).next(".group-details");
    details.toggle();
    $(this).toggleClass("active");
  });

  // Join group button
  $(document).on("click", ".JoinGroupButton", function () {
    const groupId = $(this).data("id");
    const button = $(this);

    $.ajax({
      type: "POST",
      url: `/StudyGroup/Join/${groupId}`,
      success: function (response) {
        if (response.pendingRequest) {
          button.text("Request Pending").prop("disabled", true);
        } else if (response.requiresApproval) {
          alert("Your request to join the Study Group has been sent!");
          button.text("Request Pending").prop("disabled", true);
        } else {
          alert("You have joined the Study Group!");
          button.siblings("ul").append(`<li>${response.userName}</li>`);
          button.replaceWith(
              `<button class="btn btn-danger LeaveGroupButton" data-id="${groupId}">Leave ${response.groupName}</button>`
          );
        }
      },
      error: function (xhr) {
        alert("Error joining group: " + xhr.responseText);
      },
    });
  });

  // Approve member request button
  $(document).on("click", ".ApproveMemberButton", function () {
    const groupId = $(this).data("group-id");
    const userId = $(this).data("user-id");
    const button = $(this);

    $.ajax({
      type: "POST",
      url: `/StudyGroup/ApproveMember`,
      data: { groupId: groupId, userId: userId },
      success: function (response) {
        if (response.success) {
          button.closest("li").remove();
          alert("Member approved successfully.");
        }
      },
      error: function (xhr) {
        alert("Error approving member: " + xhr.responseText);
      },
    });
  });

  // Reject member request button
  $(document).on("click", ".RejectMemberButton", function () {
    const groupId = $(this).data("group-id");
    const userId = $(this).data("user-id");
    const button = $(this);

    $.ajax({
      type: "POST",
      url: `/StudyGroup/RejectMember`,
      data: { groupId: groupId, userId: userId },
      success: function (response) {
        if (response.success) {
          button.closest("li").remove();
          alert("Member rejected successfully.");
        }
      },
      error: function (xhr) {
        alert("Error rejecting member: " + xhr.responseText);
      },
    });
  });

  // Leave group button
  $(document).on("click", ".LeaveGroupButton", function () {
    const groupId = $(this).data("id");
    const button = $(this);

    $.ajax({
      type: "POST",
      url: `/StudyGroup/Leave/${groupId}`,
      success: function (response) {
        alert("You have left the Study Group!");
        button
            .closest(".group-details")
            .find("ul")
            .find(`li:contains(${response.userName})`)
            .remove();
        button.replaceWith(
            `<button class="btn btn-primary JoinGroupButton" data-id="${groupId}">Join ${response.groupName}</button>`
        );
      },
      error: function (xhr) {
        alert("Error leaving group: " + xhr.responseText);
      },
    });
  });


  // Delete group button
  $(document).on("click", ".DeleteGroupButton", function () {
    const groupId = $(this).data("id");

    if (confirm("Are you sure you want to delete this group?")) {
      $.ajax({
        type: "POST",
        url: `/StudyGroup/Delete/${groupId}`,
        success: function (response) {
          if (response.success) {
            $(`.DeleteGroupButton[data-id='${groupId}']`)
              .closest(".col-md-4")
              .remove();
            alert("Group deleted successfully.");
          }
        },
        error: function (xhr) {
          alert("Error deleting group: " + xhr.responseText);
        },
      });
    }
  });

  // Remove member button
  $(document).on("click", ".RemoveMemberButton", function () {
    const groupId = $(this).data("group-id");
    const userId = $(this).data("user-id");
    const button = $(this);

    $.ajax({
      type: "POST",
      url: `/StudyGroup/RemoveMember`,
      data: { groupId: groupId, userId: userId },
      success: function (response) {
        if (response.success) {
          button.closest("li").remove();
          alert("Member removed successfully.");
        }
      },
      error: function (xhr) {
        alert("Error removing member: " + xhr.responseText);
      },
    });
  });

  // Search groups
  $("#searchInput").on("input", function () {
    const query = $(this).val();

    if (query === "") {
      $.ajax({
        url: "/StudyGroup/Index",
        type: "GET",
        success: function (response) {
          $("#searchResults").html($(response).find("#searchResults").html());
        },
        error: function (xhr) {
          console.error("Failed to reload groups:", xhr.responseText);
        },
      });
    } else {
      $.ajax({
        url: "/StudyGroup/Search",
        type: "GET",
        data: { query: query },
        success: function (response) {
          $("#searchResults").html(response);
        },
        error: function (xhr) {
          console.error("Search failed:", xhr.responseText);
        },
      });
    }
  });

  // Toggle approval requirement
  $("#approvalYes").click(function () {
    $("#RequiresApproval").val("true");
    $(this).addClass("btn-primary").removeClass("btn-secondary");
    $("#approvalNo").addClass("btn-secondary").removeClass("btn-primary");
  });

  $("#approvalNo").click(function () {
    $("#RequiresApproval").val("false");
    $(this).addClass("btn-primary").removeClass("btn-secondary");
    $("#approvalYes").addClass("btn-secondary").removeClass("btn-primary");
  });
});
