$(document).ready(function () {
    // Add Movie DatePicker (LastDay)
    $('#lastdaypicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        startDate: 'today',
    });
    $('#datepicker').datepicker('setDate', new Date());

    // Image Poster Upload
    const inputFile = document.querySelector("#poster-input");
    const pictureContainer = document.querySelector(".poster");
    const posterText = pictureContainer.querySelector(".poster-text");

    inputFile.addEventListener("change", function (e) {
        const inputTarget = e.target;
        const file = inputTarget.files[0];

        if (file) {
            const reader = new FileReader();

            reader.addEventListener("load", function (e) {
                const readerTarget = e.target;

                const img = document.createElement("img");
                img.src = readerTarget.result;
                img.alt = "Movie Poster";
                img.classList.add("radius", "img-fluid");
                img.style.height = "100%";
                img.style.width = "auto";

                const existingImage = pictureContainer.querySelector("img");
                if (existingImage) {
                    pictureContainer.removeChild(existingImage);
                }
                pictureContainer.appendChild(img);
                posterText.style.display = "none";
            });

            reader.readAsDataURL(file);
        } else {
            const existingImage = pictureContainer.querySelector("img");
            if (existingImage) {
                pictureContainer.removeChild(existingImage);
            }
            posterText.style.display = "block";
        }
    });


});

// Duration Conversion
$(document).ready(function () {
    function calculateTotalDuration() {
        var hours = parseInt($('#hoursSelect').val());
        var minutes = parseInt($('#minutesSelect').val());
        var totalMinutes = hours * 60 + minutes;
        $('#totalDuration').val(totalMinutes);
    }

    calculateTotalDuration();

    $('#hoursSelect, #minutesSelect').change(function () {
        calculateTotalDuration();
    });
});

// Movie Status Toggle
$(document).ready(function () {
    var checkbox = $('#isActive');

    checkbox.change(function () {
        var isActiveValue = checkbox.prop('checked') ? 1 : 0;
        $('#isActiveHidden').val(isActiveValue);
    });
});

// Add Movie
$(document).ready(function () {
    $('#addMovieForm').submit(function (event) {
        event.preventDefault();

        $('.form-control').removeClass('is-invalid');
        $('.poster').removeClass('is-invalid');
        var isValid = true;
        $('.form-control[required]').each(function () {
            if ($(this).val().trim() === '') {
                $(this).addClass('is-invalid');
                isValid = false;
            }
        });

        var posterInput = document.querySelector('#poster-input');
        if (posterInput.files.length === 0) {
            $('.poster').addClass('is-invalid');
            isValid = false;
        }

        if (!isValid) {
            return;
        }

        var formData = new FormData();
        formData.append('Title', $('#movieTitle').val());
        formData.append('Description', $('#movieDesc').val());
        formData.append('Duration', $('#totalDuration').val());
        formData.append('Rated', $('input[name="Rated"]:checked').val());
        formData.append('Price', $('#moviePrice').val());
        formData.append('LastDay', $('#lastdaypicker').val());
        formData.append('IsActive', $('#isActiveHidden').val());

        var posterInput = document.querySelector('#poster-input');
        formData.append('poster', posterInput.files[0]);

        var showtimes = $('#movieTimes').val().split(',');
        for (var i = 0; i < showtimes.length; i++) {
            formData.append('showtimes[' + i + ']', showtimes[i]);
        }

        $.ajax({
            type: 'POST',
            url: '/Admin/AddMovie',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: response.message,
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        window.location.href = '/Admin/Admin';
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.error('Error adding movie:', errorThrown);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while editing the movie. Please try again.'
                });
            }
        });
    });
});

// Edit Movie
$(document).ready(function () {
    $('#editMovieForm').submit(function (event) {
        event.preventDefault();

        var formData = new FormData();
        formData.append('MovieId', $('#movieId').val());
        formData.append('Title', $('#movieTitle').val());
        formData.append('Description', $('#movieDesc').val());
        formData.append('Price', $('#moviePrice').val());
        formData.append('LastDay', $('#lastdaypicker').val());
        formData.append('IsActive', $('#isActive').prop('checked') ? 1 : 0);

        var poster = document.querySelector('#poster-input');
        formData.append('poster', poster.files[0]);

        $.ajax({
            type: 'POST',
            url: '/Admin/EditMovie/' + movieId,
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: response.message,
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        window.location.href = '/Admin/Admin';
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.error('Error editing movie:', errorThrown);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while editing the movie. Please try again.'
                });
            }
        });
    });
});

// Delete Movie
$(document).ready(function () {
    $('.delete-movie-btn').click(function () {
        var movieId = $(this).data('movie-id');
        var url = '/Admin/DeleteMovie/' + movieId;

        Swal.fire({
            title: 'Are you sure?',
            text: 'Once deleted, you will not be able to recover this movie!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    url: url,
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success!',
                                text: response.message,
                                showConfirmButton: false,
                                timer: 2000
                            }).then(function () {
                                window.location.href = '/Admin/Admin';
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: response.message
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'An error occurred while processing the request.'
                        });
                    }
                });
            }
        });
    });
});

function toggleSeats(button) {
    var quantityLabel = document.getElementById("quantityLabel");
    var selectedButtons = document.querySelectorAll("#buttonContainer button.bg-green");
    var quantity = selectedButtons.length;

    if (button.classList.contains("bg-main")) {
        button.classList.remove("bg-main");
        button.classList.add("bg-green");
        displaySelectedValues(button.value);
        quantity++;
    } else {
        button.classList.remove("bg-green");
        button.classList.add("bg-main");
        removeSelectedValues(button.value);
        quantity--;
    }

    quantityLabel.textContent = "Quantity: " + quantity;
    updateTotalCost(unitPrice, quantity);
}

function updateTotalCost(unitPrice, quantity, discountOption) {
    var discountFactor = 1;
    if (discountOption === "pwd") {
        discountFactor = 0.8;
    } else if (discountOption === "student") {
        discountFactor = 0.9;
    }

    var totalCost = unitPrice * quantity * discountFactor;
    var totalCostElement = document.getElementById("totalCost");
    totalCostElement.textContent = "₱" + totalCost.toFixed(2);
}

function displaySelectedValues(value) {
    var selectedValuesContainer = document.getElementById("selectedValues");
    var span = document.createElement("span");
    span.className = "badge rounded-pill bg-darkshade me-2 fs-6";
    span.textContent = value;
    selectedValuesContainer.appendChild(span);
}

function removeSelectedValues(value) {
    var selectedValuesContainer = document.getElementById("selectedValues");
    var spans = selectedValuesContainer.getElementsByTagName("span");
    for (var i = 0; i < spans.length; i++) {
        if (spans[i].textContent === value) {
            spans[i].parentNode.removeChild(spans[i]);
            break;
        }
    }
}

$(document).ready(function () {
    $('#showtimes').trigger('change');
    $('#showtimes').change(function () {
        var selectedShowtime = $(this).val();

        $.ajax({
            url: '/Home/GetOccupiedSeats',
            type: 'GET',
            data: { showtime: selectedShowtime },
            success: function (data) {
                displayOccupiedSeats(data);
            },
            error: function () {
                console.error('Error fetching occupied seats.');
            }
        });
    });
});

function displayOccupiedSeats(occupiedSeats) {
    $('.bg-danger').removeClass('bg-danger').prop('disabled', false);
    occupiedSeats.forEach(function (seat) {
        var seatButton = $('.seat-button[value="' + seat + ' "]');
        seatButton.addClass('bg-danger').prop('disabled', true);
    });
}

// Purchase Ticket
function showTicketModal() {
    var movieTitle = $('#checkoutBtn').data('title');
    var moviePoster = $('#checkoutBtn').data('poster');
    var showtime = $('#showtimes').val();
    var showtimeName = $('#showtimes option:selected').text();
    var selectedSeats = $('#selectedValues').text().trim().split(/\s+/);
    var quantity = $('#quantityLabel').text().split(':')[1].trim();
    var isPWD = $('#pwd').is(':checked') ? 1 : 0;
    var isStudent = $('#student').is(':checked') ? 1 : 0;
    var unitPrice = $('#checkoutBtn').data('price'); 
    var totalCost = parseFloat($('#totalCost').text().replace('₱', '')); 

    var discountLabel = '';
    if (isPWD == 1) {
        discountLabel = 'PWD Discount: 20%';
    } else if (isStudent == 1) {
        discountLabel = 'Student Discount: 10%';
    } else {
        discountLabel = 'None';
    }

    var seatValues = '';
    for (var i = 0; i < selectedSeats.length; i++) {
        seatValues += `<span class="badge rounded-pill bg-darkshade text-lightb me-2 fs-6">${selectedSeats[i]}</span>`;
    }

    var ticketDetailsHTML = `
        <div class="row">
            <div class="col-md-4">
                <img src="${moviePoster}" class="img-fluid radius" alt="Movie Poster">
            </div>
            <div class="col-md-8">
                <h2 class="p-semibold my-2">${movieTitle}</h2>
                <p class="my-0 fst-italic">₱${unitPrice.toFixed(2)} per ticket</p>
                <p class="my-3 badge radius bg-main text-dasrkshade p-semibold px-3 fs-6">${showtimeName}</p>
                <p class="my-0 fs-6 p-semibold">Qty: ${quantity}</p>
                <div class="my-3 d-flex flex-wrap">${seatValues}</div>
                <p class="my-0 fst-italic">${discountLabel}</p>
                <h3 class="p-semibold my-4">₱${totalCost.toFixed(2)}</h3>
                <button id="purchaseBtn" class="btn bg-darkshade text-lightb radius w-100">Buy Tickets</button>
            </div>
        </div>`;

    $('#ticketDetails').html(ticketDetailsHTML);
    $('#ticketModal').modal('show');

    $('#purchaseBtn').click(function () {
        var purchaseData = {
            MovieTitle: movieTitle,
            ShowtimeId: parseInt(showtime),
            SelectedSeats: selectedSeats,
            Quantity: parseInt(quantity),
            IsPwd: isPWD,
            IsStudent: isStudent,
            TotalCost: parseFloat(totalCost)
        };

        $.ajax({
            url: '/Home/BuyTickets',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(purchaseData),
            success: function (data) {
                console.log('Response data:', data);
                Swal.fire({
                    title: 'Tickets Purchased Successfully!',
                    icon: 'success',
                    text: data.message,
                    showConfirmButton: false,
                    timer: 2000
                }).then(function () {
                    window.location.href = '/Home/TicketDetails?ticketId=' + data.ticketId;
                });
            },
            error: function (xhr, status, error) {
                console.error('Error purchasing tickets:', error);
                window.location.href = '/Home/Index';
            }
        });
    });
}



