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
    1 : 9,
    2 : 14,
    3 : 19,
    4 : 23
  }

  timeSlots = {
    9 : 1,
    14 : 2,
    19 : 3,
  }

  constructor(props) {
    super(props);

    this.state = {
      selectedDate: setHours(setMinutes(new Date(), 0), 9),
      startDate: setHours(setMinutes(new Date(), 0), 9),
      events: []
    };

  }

  handleChange = (date) => {
    this.setState({
      selectedDate: date
    })
}

  bookDate = async (data) => {
    const date = this.state.selectedDate
    const token = this.props.oidcUser.access_token
    const year = date.getFullYear()
    const month = date.getMonth() + 1
    const day = date.getDate()
    const slot = this.timeSlots[date.getHours()]
    var uri = `${configuration.api_url}/api/bookings/${year}/${month}/${day}`
    const response = await fetch(uri, {
      method: "POST",
      headers: {
          "Authorization": "Bearer "+ token,
          "Content-Type": "application/json"
      },
      body: JSON.stringify({slot:slot})
    });

    if(response.status === 201) {
      var apiResponse = await response.json()
      this.addBooking(year,month,day,apiResponse)
    }
    if(response.status === 404) {

    }
    if(response.status === 400) {
      var error = await response.json()

      alert(`error: ${error.title} - ${error.detail}`)
    }
  }

  addBooking = (year,month,day,booking) => {
    const slotHour = this.slots[booking.slot]
    const slotHourNext = this.slots[booking.slot+1]
    const events = this.state.events
    events.push({
      userId : this.props.oidcUser.profile.sub,
      start : setHours(setMinutes(new Date(year,month-1,day), 0), slotHour),
      end : setHours(setMinutes(new Date(year,month-1,day), 0), slotHourNext),
      title: `booking for ${booking.name}`
    })

    this.setState({
      events:events
    });
  }

  onView = (data) => {
    var year = data.getFullYear()
    var month = data.getMonth() + 1
    this.getBookings(year,month)
  }

  getBookingDayEvents = (year,month,day,bookingDay) =>{
    const newEvents = bookingDay.bookings.map(booking => {
      var slotHour = this.slots[booking.slot]
      var slotHourNext = this.slots[booking.slot+1]

      return {
        userId: booking.id,
        start : setHours(setMinutes(new Date(year,month-1,day), 0), slotHour),
        end : setHours(setMinutes(new Date(year,month-1,day), 0), slotHourNext),
        title: `booking for ${booking.name}`
      }
    })

    return newEvents;

  }

  setBookingMonth = (apiResponse) => {
    const bookingDays = apiResponse.bookingDays
    const year = apiResponse.year
    const month = apiResponse.month
    var newEvents = []
    Object.keys(bookingDays).forEach(key => {
      var bookingDay = bookingDays[key]
      var day = parseInt(key)
      var dayEvents = this.getBookingDayEvents(year,month,day,bookingDay)
      dayEvents.forEach(element => {
        newEvents.push(element)
      });

    });

    this.setState({
      events: newEvents
    })
  }

  getBookings = async (year,month) => {
    var token = this.props.oidcUser.access_token
    var uri = `${configuration.api_url}/api/bookings/${year}/${month}`
    const response = await fetch(uri, {
      method: "GET",
      headers: {
          "Authorization": "Bearer "+ token,
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

  eventStyleGetter = (event, start, end, isSelected) => {
    if(event.userId === this.props.oidcUser.profile.sub) {
      var style = {
        padding: '2px 5px',
        backgroundColor: '#9b4dca',
        borderRadius: '5px',
        color: '#fff',
        cursor: 'pointer'
    };
    return {
        className: "",
        style: style
    };
    }

    return {
      
    }
    
  }

  render() {
    return (
      <div className="Bookings">
        <DatePicker
          selected={this.state.selectedDate}
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
          eventPropGetter={(this.eventStyleGetter)}
          onNavigate={this.onView}
          style={{ height: "100vh" }}
          selectable={true}
        />
      </div>
    );
  }
}

export default Bookings;
