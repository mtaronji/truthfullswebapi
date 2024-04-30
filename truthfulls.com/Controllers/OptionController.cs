
using Microsoft.AspNetCore.Mvc;
using truthfulls.com.Services;


namespace truthfulls.com.Controllers
{

    [ApiController]
    public class OptionController : ControllerBase
    {

        private UtilityService _utility;
        public OptionController(UtilityService Utility) 
        {

            this._utility = Utility;
        }


    }
}
