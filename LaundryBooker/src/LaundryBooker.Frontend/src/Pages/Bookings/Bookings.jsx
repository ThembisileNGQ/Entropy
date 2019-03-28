import React, { Component } from "react";
import Calendar from "react-big-calendar";
import moment from "moment";
import DatePicker from "react-datepicker";
import setMinutes from "date-fns/setMinutes";
import setHours from "date-fns/setHours";
import configuration from "../../configuration";
import "react-datepicker/dist/react-datepicker.css";
import "react-big-calendar/lib/css/react-big-calendar.css";


const localizer = Calendar.momentLocalizer(moment);

class Bookings extends Component {
  slots = {
    9 : 1,
    14 : 2,
    19 : 3
  }
  constructor(props) {
    super(props);


    //console.log(configuration)
    //this.props.api_url = configuration.api_url
    this.state = {

      selectedDate: setHours(setMinutes(new Date(), 0), 9),
      startDate: setHours(setMinutes(new Date(), 0), 9),
      events: [
        {
          start: setHours(setMinutes(new Date(), 0), 9),
          end: setHours(setMinutes(new Date(), 0), 14),
          title: "Some title 1"
        },
        {
          start: setHours(setMinutes(new Date(), 0), 14),
          end: setHours(setMinutes(new Date(), 0), 19),
          title: "Some title 2"
        }
      ]
    };

  }

  handleChange = (date) => {
    console.log(date.getHours())
}

  bookDate = (data) => {
    console.log(this.state)
  }

  onView = (data) => {
    if(data >= setHours(setMinutes(new Date(), 0), 0)) {
      console.log(data)
    }
  }

  setBookingMonth = (apiResponse) => {
    var bookingDays = apiResponse.bookingDays
    var newEvents = []
    Object.keys(bookingDays).forEach(key => {
      console.log(bookingDays[key]);
      newEvents.push({
        //start
      })
    });
  }

  getBookings = async (year,month) => {
    var token = this.props.oidcUser.access_token
    var uri = `${configuration.api_url}/api/bookings/${year}/${month}`
    const response = await fetch(uri, {
      method: "GET", 
      headers: {
          "Authorization": "Bearer "+ token,
          // "Content-Type": "application/x-www-form-urlencoded",
      }
  });

    

    if(response.status === 200) {
      var apiResponse = await response.json()
      this.setBookingMonth(apiResponse)
    }
    if(response.status === 404) {

    }
  }
  componentDidMount() {
    var year = new Date().getFullYear()
    var month = new Date().getMonth() + 1
    this.getBookings(year,month)
  }

  render() {
    console.log(this.props)
    return (
      <div className="Bookings">
        <DatePicker
          selected={this.state.startDate}
          onChange={this.handleChange}
          minDate={new Date()}
          showTimeSelect
          includeTimes={
            [
              setHours(setMinutes(new Date(), 0), 9),
              setHours(setMinutes(new Date(), 0), 14),
              setHours(setMinutes(new Date(), 0), 19),
            ]
          }
          dateFormat="MMMM d, yyyy h:mm aa"
        />
        <button onClick={this.bookDate}>Make Booking</button>
        <Calendar
          localizer={localizer}
          defaultDate={new Date()}
          defaultView="month"
          views={['month', 'week']}
          events={this.state.events}
          onNavigate={this.onView}
          style={{ height: "100vh" }}
          selectable={true}
        />
      </div>
    );
  }
}

export default Bookings;
