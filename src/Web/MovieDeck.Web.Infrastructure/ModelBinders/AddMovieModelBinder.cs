namespace MovieDeck.Web.ModelBinders
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class AddMovieModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hrs = context.ValueProvider.GetValue("Runtime.Hours").FirstValue;
            var min = context.ValueProvider.GetValue("Runtime.Minutes").FirstValue;
            var sec = context.ValueProvider.GetValue("Runtime.Seconds").FirstValue;

            var runtime =
                TimeSpan.FromHours(int.Parse(hrs)) +
                TimeSpan.FromMinutes(int.Parse(min)) +
                TimeSpan.FromSeconds(int.Parse(sec));

            context.Result = ModelBindingResult.Success(runtime);

            return Task.CompletedTask;
        }
    }
}
