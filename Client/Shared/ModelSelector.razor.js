
export function scrollToModelSelector() {
    const modelSelectorId = "model-selector";
    const element = document.getElementById(modelSelectorId);

    if (element == null)
        return;

    element.scrollIntoView();
}

export function scrollToModelSelectorSpinner() {
    const modelSelectorSpinnerId = "model-selector-spinner";
    const element = document.getElementById(modelSelectorSpinnerId);

    if (element == null)
        return;

    element.scrollIntoView();
}
