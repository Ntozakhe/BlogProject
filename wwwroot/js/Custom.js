

let index = 0;
function AddTag() {
    //get a reference to the TagEntry input element
    var tagEntry = document.querySelector("#TagEntry");

    //lets use the new search function to detect an error
    let searchResult = search(tagEntry.value);

    if (searchResult != null) {
        //trigure sweet alert for the empty/ duplicate string
        Swal.fire({
            title: 'Error!',
            text: searchResult,
            icon: 'error',
            confirmButtonText: 'Ok',
            timer: 3000
        })
    } else {
        //create a new select Option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.querySelector("#TagList").options[index++] = newOption;        
    } 
    //Clear out the TagEntry control
    tagEntry.value = "";
    return true;
}

function DeleteTag() {
    let tagCount = 1;
    while (tagCount > 0) {
        let tagList = document.querySelector("#TagList")

        if (!tagList) return false;
        if (tagList.selectedIndex == -1) {
            //trigure sweet alert
            Swal.fire({
                title: 'Error!',
                text: 'Please CHOOSE a Tag before deleting',
                icon: 'error',
                confirmButtonText: 'Ok',
                timer: 3000
            })
            return true;
        }
        //let selectedIndex = tagList.selectedIndex;
        //above code allows the user to delete the selected index
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
            index--;
        }
    }
}

//We need to tell our select list that when our form is submitted,
//select all of the TagList so that they can make it into the post
$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
})

//Look for the tagValues variable to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        //Load up or Replace the options that we have.
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.querySelector("#TagList").options[index] = newOption;
}

//The Search function will detect either an empty or duplicate Tag
//and return an error string of an error is detected.
function search(str) {
    if (str == "") {

        return 'Empty tags are not permitted';
    }
    var tagsEl = document.querySelector("#TagList");
    if (tagsEl) {
        let options = tagsEl.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value == str) {
                return `#${str} is a duplicate and not permitted.`;
            }
        }
    }
}