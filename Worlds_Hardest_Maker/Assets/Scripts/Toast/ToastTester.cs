using System.Collections;
using UnityEngine;
using VContainer;

public class ToastTester : MonoBehaviour
{
    private IToastService toastService;
    
    [Inject]
    private void Construct(IToastService toastService)
    {
        this.toastService = toastService;
        
        toastService.ShowInfo("Toast service has been injected", 3);
    }

    private IEnumerator Start()
    {
        toastService.ShowSuccess("Toast tester started", 2);
        
        yield return new WaitForSeconds(5);
        
        toastService.ShowError("Testing error", 3);
        
        yield return new WaitForSeconds(1);
        
        toastService.ShowWarning("Testing warning", 3);
        
        yield return new WaitForSeconds(1);
        
        toastService.ShowInfo("Testing info", 3);
        
        yield return new WaitForSeconds(1);
        
        toastService.ShowSuccess("Testing success", 3);
        
        yield return new WaitForSeconds(5);
        
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
        toastService.ShowInfo("Testing spam", 3);
    }
}