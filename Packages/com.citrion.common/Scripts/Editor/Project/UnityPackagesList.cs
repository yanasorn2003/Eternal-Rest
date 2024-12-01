using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  public class UnityPackagesList : ListFromScriptableObjectItemDataCreator<ScriptableObjectItemData_PackageRemoval>
  {
    protected string addToSelectionToggleClass;
    protected string removeSelectedButtonClass;
    protected string removeSelectedWithDependenciesButtonClass;
    protected string selectAllButtonClass;
    protected string deselectAllButtonClass;
    protected string removeAssetsButtonClass;

    protected const string assetPath = "Assets/CitrioN/SMC";

    protected Action onCancelAction;

    protected List<ScriptableObjectItemData_PackageRemoval> packageDatas = new List<ScriptableObjectItemData_PackageRemoval>();

    protected static List<string> packagesToRemove = new List<string>();

    public UnityPackagesList(string groupName, VisualTreeAsset itemTemplate, VisualElement root, Action onCancel = null,
      string itemDisplayNameLabelClass = "label__item-name", string itemDescriptionNameLabelClass = "label__item-description",
      string refreshButtonClass = "button__refresh-list", string addToSelectionToggleClass = "toggle__add-to-selected",
      string removeSelectedButtonClass = "button__remove-selection",
      string removeSelectedWithDependenciesButtonClass = "button__remove-selection-with-dependencies",
      string selectAllButtonClass = "button__select-all", string deselectAllButtonClass = "button__deselect-all",
      string removeAssetsButtonClass = "button_remove-assets")
      : base(groupName, itemTemplate, root, itemDisplayNameLabelClass, itemDescriptionNameLabelClass, refreshButtonClass)
    {
      this.addToSelectionToggleClass = addToSelectionToggleClass;
      this.removeSelectedButtonClass = removeSelectedButtonClass;
      this.selectAllButtonClass = selectAllButtonClass;
      this.deselectAllButtonClass = deselectAllButtonClass;
      this.removeSelectedWithDependenciesButtonClass = removeSelectedWithDependenciesButtonClass;
      this.removeAssetsButtonClass = removeAssetsButtonClass;
      this.onCancelAction = onCancel;

      UIToolkitUtilities.SetupButton(root, "Remove selected", RemoveSelectedPackagesWithDependencies, null, removeSelectedButtonClass);
      UIToolkitUtilities.SetupButton(root, "Remove selected (incl. dependencies)", RemoveSelectedPackagesWithDependencies, null, removeSelectedWithDependenciesButtonClass);
      UIToolkitUtilities.SetupButton(root, "Remove From Assets Directory", RemoveFromAssets, null, removeAssetsButtonClass);

      UIToolkitUtilities.SetupButton(root, "Select all", SelectAll, null, selectAllButtonClass);
      UIToolkitUtilities.SetupButton(root, "Deselect all", DeselectAll, null, deselectAllButtonClass);
    }

    protected override void RefreshList()
    {
      base.RefreshList();
      if (data == null || data.Count < 1) { return; }

      string packageVersion;
      var dataToRemove = new List<ScriptableObjectItemData_PackageRemoval>();

      for (int i = 0; i < data.Count; i++)
      {
        var packageData = data[i];
        if (packageData == null) continue;

        var packageId = packageData.PackageName;
        if (string.IsNullOrEmpty(packageId)) { continue; }

        bool isInstalled = PackageUtilities.IsPackageInstalled(packageId, out packageVersion);

        if (!isInstalled)
        {
          dataToRemove.AddIfNotContains(packageData);
        }
      }

      for (int i = 0; i < dataToRemove.Count; i++)
      {
        data.Remove(dataToRemove[i]);
      }

      list?.RefreshItems();
    }

    protected override void BindListItem(VisualElement elem, int index)
    {
      base.BindListItem(elem, index);

      if (elem == null) { return; }
      if (data.Count <= index) { return; }

      var item = data[index];
      if (item == null) { return; }

      //var foldout = elem.Q<Foldout>();
      //if (foldout != null)
      //{
      //  foldout.SetText(item.DisplayName);
      //  foldout.SetValueWithoutNotify(false);
      //}

      UIToolkitUtilities.SetupToggle(elem, "Include",
        (evt) => OnValueChanged(evt, item), null, addToSelectionToggleClass);
    }

    private void OnValueChanged(ChangeEvent<bool> evt, ScriptableObjectItemData_PackageRemoval packageData)
    {
      if (packageData == null) { return; }
      if (evt.newValue == true)
      {
        packageDatas.AddIfNotContains(packageData);
      }
      else
      {
        packageDatas.Remove(packageData);
      }
    }

    private void SelectAll()
    {
      //packageDatas.Clear();
      //packageDatas.AddRange(data);
      SetAddToSelectionToggleValues(true);
    }

    private void DeselectAll()
    {
      //packageDatas.Clear();
      SetAddToSelectionToggleValues(false);
    }

    private void RemoveFromAssets()
    {
      AssetDatabase.DeleteAsset(assetPath);
    }

    private void SetAddToSelectionToggleValues(bool value)
    {
      var toggles = list?.Query<Toggle>(className: addToSelectionToggleClass).ToList();
      if (toggles == null || toggles.Count < 1) { return; }

      foreach (var toggle in toggles)
      {
        if (toggle == null) { continue; }
        toggle.value = value;
      }
    }

    private void RemoveSelectedPackagesWithoutDependencies() => RemoveSelectedPackages(false);

    private void RemoveSelectedPackagesWithDependencies() => RemoveSelectedPackages(true);

    private void RemoveSelectedPackages(bool includeDependencies)
    {
#if !NEWTONSOFT_JSON
      ConsoleLogger.LogError("Removing packages via the uninstall tab required the 'Newtonsoft Json' package to be installed!");
      return;
#else

      #region TODO Remove Later
      //var packageInfos = new List<UnityEditor.PackageManager.PackageInfo>();
      //PackageCollection packageInfos = new PackageCollection();
      //var obj = JsonConvert.DeserializeObject<List<UnityEditor.PackageManager.PackageInfo>>(json);
      //var obj = JsonConvert.DeserializeObject<UnityEditor.PackageManager.PackageInfo[]>(json);
      //var obj = JsonConvert.DeserializeObject<PackagesLockFile>(json);
      //var obj1 = JsonUtility.FromJson<PackagesLockFileCustom>(json);
      //var obj = JsonConvert.DeserializeObject<PackagesLockFileCustom>(json);

      //if (obj.dependencies.TryGetValue("com.citrion.common", out var packagevalue))
      //{
      //  var info = JsonConvert.DeserializeObject<UnityEditor.PackageInfo>(packagevalue.ToString());
      //}
      //var obj = JsonConvert.DeserializeObject(json, typeof(UnityEditor.PackageManager.PackageInfo[]));
      //var obj = JsonConvert.DeserializeObject(json, typeof(List<UnityEditor.PackageManager.PackageInfo>));
      //var ids = JsonUtility.FromJson<List<UnityEditor.PackageManager.PackageInfo>>(json);
      //var ids = JsonUtility.FromJson<PackageCollection>(json);
      //var obj = JObject.Parse(json);

      //var packages = PackageUtilities.GetAllPackages();
      //EditorJsonUtility.FromJsonOverwrite(json, packageInfos);

      //var selectedPackageDatas = packageDatas.Where(i => i != null).ToList();
      //var packageIds = new List<string>();
      //foreach (var packageData in selectedPackageDatas)
      //{
      //  packageIds.AddIfNotContains(packageData.PackageName);
      //  foreach (var d in packageData.Dependencies)
      //  {
      //    packageIds.AddIfNotContains(d);
      //  }
      //} 
      #endregion

      // Get all the packageIds selected for removal.
      // This does not include dependencies.
      var packageIds = packageDatas.Where(i => i != null).Select(i => i.PackageName).ToList();

      // Get all packages that could technically be selected
      var selectablePackageIds = data.Where(i => i != null).Select(i => i.PackageName).ToList();

      List<string> dependentPackages = new List<string>();

      if (/*includeDependencies*/true)
      {
        Dictionary<string, DependencyInfo[]> dependenciesDictionary = new Dictionary<string, DependencyInfo[]>();

        // Get all package files in the project
        List<UnityEditor.PackageManager.PackageInfo> packageJsons = AssetDatabase.FindAssets("package")
          ?.Select(AssetDatabase.GUIDToAssetPath)?.Where(i => AssetDatabase.LoadAssetAtPath<TextAsset>(i) != null)
          ?.Select(UnityEditor.PackageManager.PackageInfo.FindForAssetPath)?.ToList();

        // List of packageIds that should not be removed.
        // These are packages that for example are dependencies of packages that are not selected for removal.
        List<string> packagesNotToRemove = new List<string>();

        List<string> allDependencies = new List<string>();

        if (packageJsons != null && packageJsons.Count > 0)
        {
          // Iterate over all package files and cache their resolved dependencies
          foreach (var packageJson in packageJsons)
          {
            if (packageJson == null) { continue; }
            dependenciesDictionary.AddOrUpdateDictionaryItem(packageJson.name, packageJson.resolvedDependencies);
          }
        }

        // Iterate over all selected packageIds
        foreach (var packageId in packageIds)
        {
          if (string.IsNullOrEmpty(packageId)) { continue; }

          //dependencies = PackageUtilities.GetPackageDependencies(packageId);
          var packageJson = packageJsons?.Find(i => i != null && i.name == packageId);
          if (packageJson == null) { continue; }

          // Get resolved/nested dependencies
          var dependencies = packageJson.resolvedDependencies;

          // Iterate over all dependencies and add them to the combined list of dependencies
          foreach (var dependency in dependencies)
          {
            var dependencyName = dependency.name;
            if (!allDependencies.Contains(dependencyName) &&
                !packageIds.Contains(dependencyName) &&
                dependencyName.StartsWith("com.citrion."))
            {
              allDependencies.Add(dependencyName);
            }
          }
        }

        foreach (var packageId in selectablePackageIds)
        {
          if (string.IsNullOrEmpty(packageId)) { continue; }
          if (packageIds.Contains(packageId)) { continue; }

          var packageJson = packageJsons?.Find(i => i != null && i.name == packageId);
          if (packageJson == null) { continue; }

          allDependencies.Remove(packageId);

          // Get resolved/nested dependencies
          var dependencies = packageJson.resolvedDependencies;

          // Iterate over all dependencies and add them to the combined list of dependencies
          foreach (var dependency in dependencies)
          {
            var dependencyName = dependency.name;
            allDependencies.Remove(dependencyName);
          }
        }

        foreach (var packageId in packageIds)
        {
          if (string.IsNullOrEmpty(packageId)) { continue; }

          foreach (var entry in dependenciesDictionary)
          {
            var id = entry.Key;

            // Ignore if the package is already marked for removal
            if (packageIds.Contains(id) || allDependencies.Contains(id)) { continue; }

            var d = entry.Value;

            // Check if the dependencies of this entry package contain the dependency that should be removed.
            // If it contains the dependency it means the dependency should not be removed
            // because a package that is not being removed requires it.

            bool isRequired = false;
            for (int i = 0; i < d.Length; i++)
            {
              if (d[i].name == packageId)
              {
                isRequired = true;
                break;
              }
            }

            if (isRequired)
            {
              dependentPackages.AddIfNotContains(packageId);
              packagesNotToRemove.AddIfNotContains(packageId);
            }
          }
        }

        if (allDependencies != null && allDependencies.Count > 0)
        {
          foreach (var item in allDependencies)
          {
            foreach (var entry in dependenciesDictionary)
            {
              var id = entry.Key;

              // Ignore if the package is already marked for removal
              if (packageIds.Contains(id) || allDependencies.Contains(id)) { continue; }

              var d = entry.Value;

              // Check if the dependencies of this entry package contain the dependency that should be removed.
              // If it contains the dependency it means the dependency should not be removed
              // because a package that is not being removed requires it.

              bool isRequired = false;
              for (int i = 0; i < d.Length; i++)
              {
                if (d[i].name == item)
                {
                  isRequired = true;
                  //ConsoleLogger.LogWarning($"Can't remove {item} because {id} depends on it.");
                  break;
                }
              }

              if (isRequired)
              {
                dependentPackages.AddIfNotContains(item);
                packagesNotToRemove.AddIfNotContains(item);
              }
            }
          }

          allDependencies.RemoveAll(d => packagesNotToRemove.Contains(d));

          foreach (var item in allDependencies)
          {
            packageIds.AddIfNotContains(item);
          }
        }
      }

      //ConsoleLogger.Log($"Dependent packages {dependentPackages.Count}");

      if (packageIds == null || packageIds.Count < 1) { return; }

      packageIds.RemoveAll(p => dependentPackages.Contains(p));

      // Close the uninstall tab of the manager window.
      // This ensures if we remove all the packages that we are not left with a broken manager window.
      EditorWindow.GetWindow<ManagerWindow>().Close();

      var sb = new StringBuilder();

      if (packageIds.Count == 0 && dependentPackages.Count > 0)
      {
        sb.AppendLine("Can't remove the following packages because another package not selected for removal depends on it:");
        sb.AppendLine(string.Empty);
        dependentPackages.ForEach(p => sb.AppendLine(p));

        DialogUtilities.DisplayDialog("Invalid selection", sb.ToString(),
                                      "Go back", OnCancelPackageRemoval);
      }
      else
      {
        sb.AppendLine("The following packages will be removed from the project:");
        sb.AppendLine(string.Empty);
        packageIds.ForEach(p => sb.AppendLine(p));
        sb.AppendLine(string.Empty);
        sb.AppendLine("Click the 'Remove packages' button to start removing the packages listed above.");
        sb.AppendLine(string.Empty);
        sb.AppendLine("Click the 'Cancel removal' button to not remove any packages.");

        // Show the dialog to verify the removal of the specified packages.
        DialogUtilities.DisplayDialog("Removing packages from project", sb.ToString(),
                                      "Remove packages", "Cancel removal",
                                      () => OnAcceptPackageRemoval(packageIds),
                                      OnCancelPackageRemoval);
      }
#endif
    }

    // Old
    //private void RemoveSelectedPackages(bool includeDependencies)
    //{
    //  // Get all the packageIds selected for removal.
    //  // This does not include dependencies.
    //  var packageIds = packageDatas.Where(i => i != null).Select(i => i.StringValue).ToList();

    //  // Get all packages that could technically be selected
    //  var selectablePackageIds = data.Where(i => i != null).Select(i => i.StringValue).ToList();

    //  if (includeDependencies)
    //  {
    //    Dictionary<string, DependencyInfo[]> dependenciesDictionary = new Dictionary<string, DependencyInfo[]>();

    //    // Get all package files in the project
    //    List<UnityEditor.PackageManager.PackageInfo> packageJsons = AssetDatabase.FindAssets("package")
    //      ?.Select(AssetDatabase.GUIDToAssetPath)?.Where(i => AssetDatabase.LoadAssetAtPath<TextAsset>(i) != null)
    //      ?.Select(UnityEditor.PackageManager.PackageInfo.FindForAssetPath)?.ToList();

    //    // List of packageIds that should not be removed.
    //    // These are packages that for example are dependencies of packages that are not selected for removal.
    //    List<string> packagesNotToRemove = new List<string>();

    //    List<string> allDependencies = new List<string>();

    //    if (packageJsons != null && packageJsons.Count > 0)
    //    {
    //      // Iterate over all package files and cache their resolved dependencies
    //      foreach (var packageJson in packageJsons)
    //      {
    //        if (packageJson == null) { continue; }
    //        dependenciesDictionary.AddOrUpdateDictionaryItem(packageJson.name, packageJson.resolvedDependencies);
    //      }
    //    }

    //    // Iterate over all selected packageIds
    //    foreach (var packageId in packageIds)
    //    {
    //      if (string.IsNullOrEmpty(packageId)) { continue; }

    //      //dependencies = PackageUtilities.GetPackageDependencies(packageId);
    //      var packageJson = packageJsons?.Find(i => i != null && i.name == packageId);
    //      if (packageJson == null) { continue; }

    //      // Get resolved/nested dependencies
    //      var dependencies = packageJson.resolvedDependencies;

    //      // Iterate over all dependencies and add them to the combined list of dependencies
    //      foreach (var dependency in dependencies)
    //      {
    //        allDependencies.AddIfNotContains(dependency.name);
    //      }

    //      #region Later

    //      //foreach (var dependency in dependencies)
    //      //{
    //      //  var dependencyPackageId = dependency.name;

    //      //  // Ignore if the dependency package is also selected
    //      //  if (packageIds.Contains(dependencyPackageId)) { continue; }

    //      //  // Ignore if it is already in the list of dependency packages to remove
    //      //  if (dependencyPackagesIds.Contains(dependencyPackageId)) { continue; }

    //      //  // Ignore if the package should not be removed
    //      //  if (packagesNotToRemove.Contains(dependencyPackageId)) { continue; }

    //      //  // Ignore if the package is not a selectable package.
    //      //  // Only packages that can also manually be selected should be allowed for removal.
    //      //  // This prevents unnecessary removal of important packages.
    //      //  bool isSelectablePackage = data.Find(d => d.StringValue == dependencyPackageId) != null;
    //      //  if (!isSelectablePackage) { continue; }

    //      //  bool isDependencyOfPackageNotIncluded = false;

    //      //  // Iterate over all package dependencies to check if a package depends on it
    //      //  foreach (var package in dependenciesDictionary)
    //      //  {
    //      //    var id = package.Key;

    //      //    // Ignore checking for dependencies if the package is already being removed
    //      //    if (packageIds.Contains(id)) { continue; }

    //      //    // Ignore if it was already added as a dependency to remove
    //      //    if (dependencyPackagesIds.Contains(id)) { continue; }

    //      //    // Ignore if it should already not be removed
    //      //    if (packagesNotToRemove.Contains(id)) { continue; }

    //      //    // Check if the dependency package is not a selectable package
    //      //    isSelectablePackage = data.Find(i => i.StringValue == id) != null;
    //      //    if (!isSelectablePackage)
    //      //    {
    //      //      packagesNotToRemove.Add(id);
    //      //      continue;
    //      //    }

    //      //    // Ignore if there are no valid dependencies
    //      //    if (package.Value == null || package.Value.Length < 1) { continue; }

    //      //    bool packageDependsOnPriorDependency = false;

    //      //    // Check if the package iterated over depends on the prior dependency
    //      //    for (int i = 0; i < package.Value.Length; i++)
    //      //    {
    //      //      var d = package.Value[0];
    //      //      if (d.name == dependencyPackageId && dependencyPackageId )
    //      //      {
    //      //        packageDependsOnPriorDependency = true;
    //      //        break;
    //      //      }
    //      //    }

    //      //    if (!packageDependsOnPriorDependency)
    //      //    {

    //      //    }
    //      //  }

    //      //  dependencyPackagesIds.Add(dependencyPackageId);
    //      //}

    //      /////////

    //      //if (dependenciesDictionary.TryGetValue(packageId, out var dependencyInfos))
    //      //{
    //      //  // Iterate over all dependencies of this package
    //      //  // Add all dependency packages to the list of packages to remove
    //      //  // if no other package that will not be removed does depend on it.
    //      //  var sb = new StringBuilder();
    //      //  sb.AppendLine($"Dependencies for {packageId}");
    //      //  for (int i = 0; i < dependencyInfos.Length; i++)
    //      //  {
    //      //    var di = dependencyInfos[i];
    //      //    sb.AppendLine(di.name);
    //      //  }
    //      //  Debug.Log(sb.ToString());
    //      //} 
    //      #endregion
    //    }

    //    if (allDependencies != null && allDependencies.Count > 0)
    //    {
    //      // Remove dependency packages that are already selected
    //      allDependencies.RemoveAll(d => packageIds.Contains(d));

    //      // Remove dependency packages that can not be selected
    //      allDependencies.RemoveAll(d => !selectablePackageIds.Contains(d));

    //      foreach (var item in allDependencies)
    //      {
    //        foreach (var entry in dependenciesDictionary)
    //        {
    //          var id = entry.Key;

    //          // Ignore if the package is already marked for removal
    //          if (packageIds.Contains(id) || allDependencies.Contains(id)) { continue; }

    //          var d = entry.Value;

    //          // Check if the dependencies of this entry package contain the dependency that should be removed.
    //          // If it contains the dependency it means the dependency should not be removed
    //          // because a package that is not being removed requires it.

    //          bool isRequired = false;
    //          for (int i = 0; i < d.Length; i++)
    //          {
    //            if (d[i].name == item)
    //            {
    //              isRequired = true;
    //              break;
    //            }
    //          }

    //          if (isRequired)
    //          {
    //            packagesNotToRemove.AddIfNotContains(item);
    //          }
    //        }
    //      }

    //      allDependencies.RemoveAll(d => packagesNotToRemove.Contains(d));

    //      foreach (var item in allDependencies)
    //      {
    //        packageIds.AddIfNotContains(item);
    //      }
    //    }
    //  }

    //  if (packageIds == null || packageIds.Count < 1) { return; }

    //  // Close the uninstall tab of the manager window.
    //  // This ensures if we remove all the packages that we are not left with a broken manager window.
    //  EditorWindow.GetWindow<ManagerWindow>().Close();

    //  var sb = new StringBuilder();
    //  sb.AppendLine("The following packages will be removed from the project:");
    //  sb.AppendLine(string.Empty);
    //  packageIds.ForEach(p => sb.AppendLine(p));
    //  sb.AppendLine(string.Empty);
    //  sb.AppendLine("Click the 'Remove packages' button to start removing the packages listed above.");
    //  sb.AppendLine(string.Empty);
    //  sb.AppendLine("Click the 'Cancel removal' button to not remove any packages.");

    //  // Show the dialog to verify the removal of the specified packages.
    //  DialogUtilities.DisplayDialog("Removing packages from project", sb.ToString(),
    //                                "Remove packages", "Cancel removal",
    //                                () => OnAcceptPackageRemoval(packageIds),
    //                                OnCancelPackageRemoval);
    //}

    private void OnAcceptPackageRemoval(List<string> packageIds)
    {
      PackageUtilities.RemovePackages(packageIds);
    }

    private void OnCancelPackageRemoval()
      => onCancelAction?.Invoke();
  }
}
