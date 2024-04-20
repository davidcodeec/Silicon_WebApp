document.addEventListener('DOMContentLoaded', function () {
    handleProfileImageUpload()
})


function handleProfileImageUpload() {
    try {
        let fileUploader = document.querySelector('#fileUploader');
        if (fileUploader !== undefined && fileUploader !== null) {
            fileUploader.addEventListener('change', function () {
                if (this.files.length > 0) {
                    this.closest('form').submit();
                }
            });
        }
    } catch (error) {
        console.error('Error handling profile image upload:', error);
    }
}



document.addEventListener('DOMContentLoaded', function () {
    let sw = document.querySelector('#switch-mode')

    sw.addEventListener('change', function () {
        let theme = this.checked ? "dark" : "light"

        fetch(`/sitesettings/changetheme?mode=${theme}`)
            .then(res => {
                if (res.ok)
                    window.location.reload();
                else
                    console.log('something went wrong')
            })
    })
})






function searchQuery() {
    const searchInput = document.querySelector('#searchQuery');

    // Check if search input is available
    if (!searchInput) {
        console.error("Search input not found.");
        return;
    }

    // Trigger updateCoursesByFilters when the input value changes
    searchInput.addEventListener('input', function () {
        const selectedCategory = document.getElementById('categorySelect').value;
        updateCoursesByFilters(selectedCategory);
    });
}






function updateCoursesByFilters(category) {
    const searchQuery = encodeURIComponent(document.querySelector("#searchQuery").value);
    const encodedCategory = encodeURIComponent(category === 'All' ? 'all' : category);

    const token = getCookie("AccessToken");

    // Check if token is available
    if (!token) {
        // Redirect the user to the sign-in page or display an error message
        // Example: window.location.href = '/signin'; 
        return;
    }

    const apiKey = "";
    let url = `https://localhost:7223/api/Courses?category=${encodedCategory}&searchQuery=${searchQuery}&key=${apiKey}`;

    // Make the fetch request with the bearer token in the Authorization header
    fetch(url, {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(res => {
            if (res.status === 401) {
                throw new Error("Unauthorized");
            }
            return res.json();
        })
        .then(data => {
            // Handle the response data
            console.log("Response data:", data);
            const imageUrl = '/images/courses';
            renderCourses(data.courses, imageUrl);

            // Update pagination regardless of the category selected
            const paginationHTML = data.pagination ? data.pagination : '';
            document.querySelector('.pagination').innerHTML = paginationHTML;
        })
        .catch(error => {
            console.error('Error fetching courses:', error);
            // Handle other errors if needed
        });
}









function getCookie(name) {
    const cookies = document.cookie.split(';'); // Split all cookies into an array
    for (let cookie of cookies) {
        const [cookieName, cookieValue] = cookie.split('=').map(c => c.trim()); // Split each cookie into name and value
        if (cookieName === name) {
            return decodeURIComponent(cookieValue); // Return the value if the cookie name matches
        }
    }
    return null; // Return null if the cookie with the given name is not found
}



function populateCategories(categoriesData) {
    const selectElement = document.getElementById('categorySelect');
    selectElement.innerHTML = ''; // Clear existing options

    // Add 'All Categories' option
    const allCategoriesOption = document.createElement('option');
    allCategoriesOption.value = 'All';
    allCategoriesOption.textContent = 'All Categories';
    selectElement.appendChild(allCategoriesOption);

    // Add options for each category
    categoriesData.forEach(category => {
        const option = document.createElement('option');
        option.value = category.CategoryName;
        option.textContent = category.CategoryName;
        selectElement.appendChild(option);
    });
}



function renderCourses(courses, imageUrl) {
    const courseItemsContainer = document.querySelector('.course-items');

    if (courseItemsContainer) {
        courseItemsContainer.innerHTML = ''; // Clear existing content

        if (Array.isArray(courses)) {
            courses.forEach(course => {
                const courseItem = document.createElement('div');
                courseItem.classList.add('course');
                courseItem.setAttribute('onclick', `location.href='${course.detailsUrl}'`);

                // Construct the image URL
                const imageUrlWithPath = `${imageUrl}/${course.image}`;

                // Construct the HTML for the course item
                courseItem.innerHTML = `
                    <img src="${imageUrlWithPath}" alt="${course.title}" /> 
                    <div class="content">
                        <h3 class="title">${course.title}</h3>
                        <p class="author">By ${course.author}</p>
                        <div class="pricing">
                            ${course.discountPrice ? `<div class="discount">${course.discountPrice}</div>` : ''}
                            <div class="price ${course.discountPrice ? 'discount-enabled' : ''}">${course.price}</div>
                        </div>
                        <div class="footer">
                            <div class="hours">
                                <i class="fa-regular fa-clock">${course.hours} hours</i>
                            </div>
                            <div class="likes">
                                <i class="fa-regular fa-thumbs-up"></i> ${course.likes} (${course.likePercentage}%)
                            </div>
                        </div>
                    </div>
                `;

                // Append the course item to the container
                courseItemsContainer.appendChild(courseItem);
            });
        } else {
            console.error('Error: Courses data is not an array');
        }
    } else {
        console.error('Error: .course-items container not found in the current document');
    }
}




document.addEventListener('DOMContentLoaded', function () {
    const selectElement = document.getElementById('categorySelect');
    const searchInput = document.getElementById('searchQuery');

    // Check if the selectElement exists before attaching the event listener
    if (selectElement) {
        selectElement.addEventListener('change', function () {
            const selectedCategory = selectElement.value;
            updateCoursesByFilters(selectedCategory);
        });
    }

    // Trigger updateCoursesByFilters when the input value changes
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const selectedCategory = selectElement.value;
            updateCoursesByFilters(selectedCategory);
        });
    }

    // Call the searchQuery function
    searchQuery();
});



























