using Gym.Business.Common;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Extensions;

public static class ResultControllerExtensions
{
    public static IActionResult ViewIndexOrEmpty<T>(
        this Controller controller,
        Result<IEnumerable<T>> result,
        string fallbackError)
    {
        if (result.IsFailure)
        {
            controller.TempData["Error"] = !string.IsNullOrWhiteSpace(result.Error.Description)
                ? result.Error.Description
                : fallbackError;

            return controller.View(Enumerable.Empty<T>());
        }

        return controller.View(result.Value);
    }

    public static IActionResult ViewIndexOrEmpty<T>(
        this Controller controller,
        Result<IReadOnlyList<T>> result,
        string fallbackError)
    {
        if (result.IsFailure)
        {
            controller.TempData["Error"] = !string.IsNullOrWhiteSpace(result.Error.Description)
                ? result.Error.Description
                : fallbackError;

            return controller.View(Array.Empty<T>());
        }

        return controller.View(result.Value);
    }

    public static IActionResult ViewOrNotFound<T>(this Controller controller, Result<T> result)
    {
        if (result.IsFailure)
            return controller.NotFound();

        return controller.View(result.Value);
    }

    public static IActionResult ViewDetailsOrRedirect<T>(
        this Controller controller,
        Result<T> result,
        string fallbackError)
    {
        if (result.IsFailure)
            return controller.RedirectIndexOnFailure(result, fallbackError);

        return controller.View(result.Value);
    }

    public static IActionResult ViewFormOnFailure<T>(
        this Controller controller,
        Result result,
        T model,
        string tempDataError)
    {
        controller.AddFailureToModelState(result);
        controller.TempData["Error"] = tempDataError;
        return controller.View(model);
    }

    public static IActionResult RedirectIndexOnFailure(
        this Controller controller,
        Result result,
        string fallbackError)
    {
        controller.TempData["Error"] = !string.IsNullOrWhiteSpace(result.Error.Description)
            ? result.Error.Description
            : fallbackError;

        return controller.RedirectToAction("Index");
    }

    public static void AddFailureToModelState(
        this Controller controller,
        Result result)
    {
        controller.ModelState.AddModelError(result.Error.Code, result.Error.Description);
    }
}
