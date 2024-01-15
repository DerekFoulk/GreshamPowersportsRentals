
export function scrollToModelSelector() {
    const modelSelectorId = "model-selector";

    console.log(`Scrolling to '#${modelSelectorId}'`);

    const element = document.getElementById(modelSelectorId);

    element.scrollIntoView();

    console.log(`'#${modelSelectorId}' was scrolled into view`);
}
