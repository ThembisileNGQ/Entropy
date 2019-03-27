import React, { Component } from "react";
import Calendar from "react-big-calendar";
import moment from "moment";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";



import "react-big-calendar/lib/css/react-big-calendar.css";

const localizer = Calendar.momentLocalizer(moment);

class Bookings extends Component {

  state = {
    events: [
      {
        start: new Date(),
        end: new Date(moment().add(1, "days")),
        title: "Some title"
      }
    ]
  };

  render() {
    return (
      <div className="Bookings">
        <Calendar
          localizer={localizer}
          defaultDate={new Date()}
          defaultView="month"
          views={['month', 'week']}
          events={this.state.events}
          style={{ height: "100vh" }}
          selectable="true"
        />
      </div>
    );
  }
}

export default Bookings;
