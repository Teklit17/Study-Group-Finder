document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        selectable: true,
        editable: true,
        events: '/api/events', // Endpoint to fetch events
        select: function (info) {
            var title = prompt('Enter Event Title');
            var description = prompt('Enter Event Description');
            var location = prompt('Enter Event Location');
            if (title) {
                var eventData = {
                    title: title,
                    start: info.startStr,
                    end: info.endStr,
                    allDay: info.allDay,
                    description: description,
                    location: location
                };
                calendar.addEvent(eventData);
                fetch('/api/events', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(eventData)
                }).then(response => {
                    if (response.ok) {
                        alert('Event added successfully');
                    } else {
                        alert('Failed to add event');
                    }
                });
            }
            calendar.unselect();
        },
        eventClick: function (info) {
            var eventObj = info.event;
            var newTitle = prompt('Edit Event Title', eventObj.title);
            if (newTitle !== null) {
                eventObj.setProp('title', newTitle);
                fetch(`/api/events/${eventObj.id}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ title: newTitle })
                }).then(response => {
                    if (response.ok) {
                        alert('Event updated successfully');
                    } else {
                        alert('Failed to update event');
                    }
                });
            }
        },
        eventRemove: function (info) {
            if (confirm('Are you sure you want to delete this event?')) {
                fetch(`/api/events/${info.event.id}`, {
                    method: 'DELETE'
                }).then(response => {
                    if (response.ok) {
                        alert('Event deleted successfully');
                    } else {
                        alert('Failed to delete event');
                    }
                });
            }
        }
    });
    calendar.render();
});