//run the function on page load:
document.onload = setPreviousUrl();

function setPreviousUrl() {
	//get the actual & previous page
	const last_page = document.referrer;
	const current_page = window.location.href;

	//remove history when entering index or checkout validtion page
	if (
		current_page == "http://127.0.0.1:5000/index" ||
		current_page == "http://127.0.0.1:5000/" ||
		current_page == "http://127.0.0.1:5000/checkout/verify"
	) {
		if (sessionStorage.getItem("history_list")) {
			sessionStorage.removeItem("history_list");
		}
		if (sessionStorage.getItem("goback_used")) {
			sessionStorage.removeItem("goback_used");
		}
		return;
	}
	// Do not keep track of the last url if we used the go back button
	// to avoid going back to it again and again
	if (sessionStorage.getItem("goback_used")) {
		sessionStorage.removeItem("goback_used");
		return;
	}

	//If there is something in the history, get it:
	if (sessionStorage.getItem("history_list")) {
		const history_list = JSON.parse(sessionStorage.getItem("history_list"));
		let last_history_element = history_list[history_list.length - 1];

		// Then append the new link to history if its different from current page link
		// and last link in history
		if (last_page !== last_history_element && last_page !== current_page) {
			history_list.push(last_page);
			// Keep the history length decent
			if (history_list.length > 20) {
				history_list.shift();
			}
			// Save changes
			sessionStorage.setItem("history_list", JSON.stringify(history_list));
		} else return;
		//Else create the history:
	} else {
		const history_list = [];
		history_list.push(last_page);
		sessionStorage.setItem("history_list", JSON.stringify(history_list));
	}
}

function goBack() {
	const history_list = JSON.parse(sessionStorage.getItem("history_list"));
	let last_history_element = history_list[history_list.length - 1];
	//Remove last_history_element from history array
	history_list.pop();
	if (history_list.length == 0) {
		//If history is empty, remove it from session storage
		sessionStorage.removeItem("history_list");
	} else sessionStorage.setItem("history_list", JSON.stringify(history_list));

	if (last_history_element) {
		// Add a goback_used item to avoid looping between last two urls
		sessionStorage.setItem("goback_used", true);
		window.location.replace(last_history_element);
	} else return;
}

// Add the goback function to the back button
const backButton = document.getElementById("backButton");

// Remove the button display if there is no history
if (sessionStorage.getItem("history_list") == null) {
	backButton.classList.add("d-none");
} else backButton.classList.remove("d-none");

backButton.addEventListener("click", goBack);
